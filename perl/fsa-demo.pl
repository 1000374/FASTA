use Bio::Trace::ABIF;

#Functiondefinition.
#$tag_name 	单个标签名字
#$tag_num   单个标签序号
#$file     	写入的文件路径
#@value    	写入的值
#@result 	成功返回 1 失败返回 0
sub get_value {
	my ( $tag_name, $tag_num, @value ) = @_;

	#判断列表元素个数 1--标量  n--引用
	$len = @value;
	print "===\$tag_name=$tag_name===";
#	print "\$tag_num=$tag_num\n";
#	print "\@value=@value\n";
#	print "\$len=$len----------------------------------------\n";
	if ( $len == 1 ) {

		$Val = shift(@value);    #取得第一个元素

		#		print "\$Val=$Val\n";
		$Val =~ s/[\r\n]$//;

		#		print "\$Val=$Val\n";
		@result = $abif->write_tag( $tag_name, $tag_num, $Val );   #替换标量

		#		$tag_name = "RUNT";
		#		@result = $abif->write_tag( $tag_name, 3, $Val );
	}
	else {
		@result =
		  $abif->write_tag( $tag_name, $tag_num, \@value ); # Pass by reference!
	}

	return @result;
}

#$tag_name 标签名
#$txtfile  数据路径
#$fsafile  写入的FSA文件
#$result   返回替换成功的个数
sub readfile1 {
	my ( $tag_name, $txtfile, $fsafile ) = @_;
	
	$abif = Bio::Trace::ABIF->new();
	$abif->open_abif( $fsafile, 1 );
#	print "\$fsafile=$fsafile\n";
	open( F, $txtfile ) || die("Can't open file $txtfile for read!");

	my $result   = 0;
	my @tag_name = split( ",", $tag_name );      # 拆分标签数组
	my @RUNDT    = ( 1, 2, 3, 4, 1, 2, 3, 4 );
	my @DyeNW    = ();
	my @DATA     = ();

	foreach $tag (@tag_name) {
		$read_line = <F>;                        #逐行读取
		my @qv = split( ",", $read_line );
		if ( $tag eq "Dye#" ) {                  # 根据几色添加数组
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
		else {    #tag序号为1的标签
			$result += get_value( $tag, 1, @qv );
		}

	}

	return $result;
}


#$txtfile  数据路径
#$fsafile  写入的FSA文件
#$result   返回替换成功的个数
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

	while ( $read_line = <F> ) {    #逐行读取
		my @tag_line = split( "]", $read_line );    # 拆分标签和数据
#		print("\@tag_line=@tag_line\n");
		$tag = substr( shift(@tag_line), 1 );       #标签
#		print("\$tag=$tag\n");
		my @qv = split( ",", shift(@tag_line) );
#		print("\@qv=@qv\n");
		if ( $tag eq "Dye#" ) {                     # 根据几色添加数组
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
		elsif ( $tag eq "DyeB" || $tag eq "MODL" ) {# 去除不可写的标签及不需要的标签 $tag eq "CTID" || $tag eq "CTNM" || $tag eq "CTOw"|| $tag eq "RunN"||
			print("==$tag标签不可写或不需要写==");
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
		else {    #tag序号为1的标签
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

#测试用代码
# 问题标签 CTID,CTNM,RunN,CTOw
#$p1 = "C:\\Users\\11\\Desktop\\fsa_file_demo\\fsa_file\\fsa-demo.txt";
#$p2 = "C:\\Users\\11\\Desktop\\fsa_file_demo\\fsa\\temp-05.fsa";
#$result = readfile( $p1, $p2 );
#print "result=$result\n";

#######printresultis:
