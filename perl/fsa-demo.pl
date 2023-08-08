use Bio::Trace::ABIF;

#Functiondefinition.
#$tag_name 	������ǩ����
#$tag_num   ������ǩ���
#$file     	д����ļ�·��
#@value    	д���ֵ
#@result 	�ɹ����� 1 ʧ�ܷ��� 0
sub get_value {
	my ( $tag_name, $tag_num, @value ) = @_;

	#�ж��б�Ԫ�ظ��� 1--����  n--����
	$len = @value;
	print "===\$tag_name=$tag_name===";
#	print "\$tag_num=$tag_num\n";
#	print "\@value=@value\n";
#	print "\$len=$len----------------------------------------\n";
	if ( $len == 1 ) {

		$Val = shift(@value);    #ȡ�õ�һ��Ԫ��

		#		print "\$Val=$Val\n";
		$Val =~ s/[\r\n]$//;

		#		print "\$Val=$Val\n";
		@result = $abif->write_tag( $tag_name, $tag_num, $Val );   #�滻����

		#		$tag_name = "RUNT";
		#		@result = $abif->write_tag( $tag_name, 3, $Val );
	}
	else {
		@result =
		  $abif->write_tag( $tag_name, $tag_num, \@value ); # Pass by reference!
	}

	return @result;
}

#$tag_name ��ǩ��
#$txtfile  ����·��
#$fsafile  д���FSA�ļ�
#$result   �����滻�ɹ��ĸ���
sub readfile1 {
	my ( $tag_name, $txtfile, $fsafile ) = @_;
	
	$abif = Bio::Trace::ABIF->new();
	$abif->open_abif( $fsafile, 1 );
#	print "\$fsafile=$fsafile\n";
	open( F, $txtfile ) || die("Can't open file $txtfile for read!");

	my $result   = 0;
	my @tag_name = split( ",", $tag_name );      # ��ֱ�ǩ����
	my @RUNDT    = ( 1, 2, 3, 4, 1, 2, 3, 4 );
	my @DyeNW    = ();
	my @DATA     = ();

	foreach $tag (@tag_name) {
		$read_line = <F>;                        #���ж�ȡ
		my @qv = split( ",", $read_line );
		if ( $tag eq "Dye#" ) {                  # ���ݼ�ɫ�������
			$result += get_value( $tag, 1, @qv );
			my @DyeN    = ( 1, 2, 3, 4 );
			my @CCDData = ( 1, 2, 3, 4 );
			my @EPT     = ( 5, 6, 7, 8 );
			if ( $read_line == 4 ) {
				@DATA  = ( @CCDData, @EPT );
				@DyeNW = ( @DyeN,    @DyeN );
			}
			elsif ( $read_line == 5 ) {
				push( @CCDData, 105 );
				@DATA = ( @CCDData, @EPT );
				push( @DyeN, 5 );
				@DyeNW = ( @DyeN, @DyeN );
			}
			elsif ( $read_line == 6 ) {
				push( @CCDData, 105 );
				push( @CCDData, 106 );
				@DATA = ( @CCDData, @EPT );
				push( @DyeN, 5 );
				push( @DyeN, 6 );
				@DyeNW = ( @DyeN, @DyeN );
			}

		}
		elsif ( $tag eq "DyeN" || $tag eq "DyeW" ) {
			my $tag_cou = shift(@DyeNW);
			$result += get_value( $tag, $tag_cou, @qv );
		}
		elsif ( $tag eq "RUND" || $tag eq "RUNT" ) {
			my $tag_cou = shift(@RUNDT);
			$result += get_value( $tag, $tag_cou, @qv );
		}
		elsif ( $tag eq "DATA" ) {
			my $tag_cou = shift(@DATA);
			$result += get_value( $tag, $tag_cou, @qv );
		}
		else {    #tag���Ϊ1�ı�ǩ
			$result += get_value( $tag, 1, @qv );
		}

	}

	return $result;
}


#$txtfile  ����·��
#$fsafile  д���FSA�ļ�
#$result   �����滻�ɹ��ĸ���
sub readfile {
	my ( $txtfile, $fsafile ) = @_;
	$abif = Bio::Trace::ABIF->new();
	$abif->open_abif( $fsafile, 1 );
#	print "\$fsafile=$fsafile\n";
	open( F, $txtfile ) || die("Can't open file $txtfile for read!");
	my $result = 0;
	my @RUNDT  = ( 1, 2, 3, 4, 1, 2, 3, 4 );
	my @DyeNW  = ();
	my @DATA   = ();

	while ( $read_line = <F> ) {    #���ж�ȡ
		my @tag_line = split( "]", $read_line );    # ��ֱ�ǩ������
#		print("\@tag_line=@tag_line\n");
		$tag = substr( shift(@tag_line), 1 );       #��ǩ
#		print("\$tag=$tag\n");
		my @qv = split( ",", shift(@tag_line) );
#		print("\@qv=@qv\n");
		if ( $tag eq "Dye#" ) {                     # ���ݼ�ɫ�������
			$result += get_value( $tag, 1, @qv );
			my @DyeN    = ( 1, 2, 3, 4 );
			my @CCDData = ( 1, 2, 3, 4 );
			my @EPT     = ( 5, 6, 7, 8 );

			my $dyec = shift(@qv);
#			print("\$dyec=$dyec\n");
			if ( $dyec == 4 ) {
				@DATA  = ( @CCDData, @EPT );
				@DyeNW = ( @DyeN,    @DyeN );
			}
			elsif ( $dyec == 5 ) {
				push( @CCDData, 105 );
				@DATA = ( @CCDData, @EPT );
				push( @DyeN, 5 );
				@DyeNW = ( @DyeN, @DyeN );
			}
			elsif ( $dyec == 6 ) {
				push( @CCDData, 105 );
				push( @CCDData, 106 );
				@DATA = ( @CCDData, @EPT );
				push( @DyeN, 5 );
				push( @DyeN, 6 );
				@DyeNW = ( @DyeN, @DyeN );
			}

		}
		elsif ( $tag eq "DyeB" || $tag eq "MODL" ) {# ȥ������д�ı�ǩ������Ҫ�ı�ǩ $tag eq "CTID" || $tag eq "CTNM" || $tag eq "CTOw"|| $tag eq "RunN"||
			print("==$tag��ǩ����д����Ҫд==");
		}
		elsif ( $tag eq "DyeN" || $tag eq "DyeW" ) {
			my $tag_cou = shift(@DyeNW);
			$result += get_value( $tag, $tag_cou, @qv );
		}
		elsif ( $tag eq "RUND" || $tag eq "RUNT" ) {
			my $tag_cou = shift(@RUNDT);
			$result += get_value( $tag, $tag_cou, @qv );
		}
		elsif ( $tag eq "DATA" ) {
			my $tag_cou = shift(@DATA);
			$result += get_value( $tag, $tag_cou, @qv );
		}
		else {    #tag���Ϊ1�ı�ǩ
			$result += get_value( $tag, 1, @qv );
		}

	}

	return $result;
}

#public static String exec(String exec,String function,String tag_name, String tag_num,String values,String file){
#########Mainscript
#my $result;
#$ARGV[1] txtFile
#$ARGV[2] fsaFile
$result = readfile( $ARGV[0], $ARGV[1] );
print "result=$result\n";

#�����ô���
# �����ǩ CTID,CTNM,RunN,CTOw
#$p1 = "C:\\Users\\11\\Desktop\\fsa_file_demo\\fsa_file\\fsa-demo.txt";
#$p2 = "C:\\Users\\11\\Desktop\\fsa_file_demo\\fsa\\temp-05.fsa";
#$result = readfile( $p1, $p2 );
#print "result=$result\n";

#######printresultis:
