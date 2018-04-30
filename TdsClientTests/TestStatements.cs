using System;

namespace TdsClientTests
{
    public static class TestStatements
    {
        public static string NotNullTable = @"
        DECLARE @NotNullTable as Table 
        (
	        nId				    int identity primary key, 
	        ndate               date			 not null,
	        ntime               time			 not null,
	        ndatetime2          datetime2		 not null,
	        ndatetimeoffset     datetimeoffset	 not null,
	        nbit                bit				 not null,
	        ntinyint            tinyint			 not null,
	        nsmallint           smallint		 not null,
	        nint                int				 not null,
	        nbigint             bigint			 not null,
	        nreal               real			 not null,
	        nfloat              float			 not null,
	        nmoney              money			 not null,
	        nsmallmoney         smallmoney		 not null,
	        nsmalldatetime      smalldatetime	 not null,
	        ndatetime           datetime		 not null,
	        nuniqueidentifier   uniqueidentifier not null,
	        nbinary             binary(1)		 not null,
	        nvarbinary          varbinary(max)	 not null,
	        nchar               char(1)			 not null,
	        nvarchar            varchar(max)	 not null,
	        nnchar              nchar(1)		 not null,
	        nnvarchar           nvarchar(max)	 not null,
	        nXML                XML 			 not null,
	        ndecimal4           decimal(4,1)	 not null,
	        ndecimal8           decimal(38,0)	 not null,
	        ndecimal12          decimal(38,0)	 not null,
	        ntimestamp          timestamp		 not null,
            ntext               text             not null,
            nntext              ntext            not null,
            nvarbinarysmall     binary varying   not null,--seems not available is varbinary(max)
            nimage              image            not null
        );

        INSERT @NotNullTable (
	         ndate            
	        ,ntime            
	        ,ndatetime2       
	        ,ndatetimeoffset  
	        ,nbit             
	        ,ntinyint         
	        ,nsmallint        
	        ,nint             
	        ,nbigint          
	        ,nreal            
	        ,nfloat           
	        ,nmoney           
	        ,nsmallmoney      
	        ,nsmalldatetime   
	        ,ndatetime        
	        ,nuniqueidentifier
	        ,nbinary          
	        ,nvarbinary       
	        ,nchar            
	        ,nvarchar         
	        ,nnchar           
	        ,nnvarchar        
	        ,nXML             
	        ,ndecimal4        
	        ,ndecimal8        
	        ,ndecimal12       
	        ,ntext       
	        ,nntext
	        ,nvarbinarysmall
	        ,nimage
        )
        VALUES(
	         '20181231'
	        ,'10:11:12'
	        ,'20181231 10:11:12'
	        ,'20181231 10:11:12+5:00'
	        ,1
	        ,1
	        ,1
	        ,1
	        ,1
	        ,1
	        ,1
	        ,1
	        ,1
	        ,'20181231'
	        ,'20181231'
	        ,cast('9e383328-69d7-4e73-8126-e25a1be94ae9' as uniqueidentifier)
	        ,CAST( 1 AS BINARY(1) )
	        ,CAST( replicate('123456789',1) AS varbinary(max) )
	        ,CAST( 1 AS char(1) )
	        ,CAST( replicate('123456789',1) AS varchar(max) )
	        ,CAST( 1 AS nchar(1) )
	        ,CAST( replicate('123456789',1) AS nvarchar(max) )
	        ,'<Data><DepartmentID>{x}</DepartmentID></Data>'
	        ,1
	        ,power(10.0,18)
	        ,power(10.0,28)
	        ,'1'
	        ,'1'
            ,CAST( 1 AS VARBINARY(1) )
            ,0x1
        )
        SELECT * FROM @notNullTable";

        public static string SelectAllNullTypes = @"
            DECLARE @a date
            DECLARE @b time
            DECLARE @c datetime2
            DECLARE @d datetimeoffset
            DECLARE @e bit
            DECLARE @f tinyint
            DECLARE @g smallint
            DECLARE @h int
            DECLARE @i bigint
            DECLARE @j real
            DECLARE @k float
            DECLARE @l money
            DECLARE @m smallmoney
            DECLARE @n smalldatetime
            DECLARE @o datetime
            DECLARE @p uniqueidentifier
            DECLARE @q BINARY(1)
            DECLARE @r varbinary(max)
            DECLARE @s char(1)
            DECLARE @t varchar(max)
            DECLARE @u nchar(1)
            DECLARE @v nvarchar(max)
            DECLARE @w XML 
            DECLARE @x decimal(4,1)
            DECLARE @y decimal(38,0)
            DECLARE @z decimal(38,0)
            DECLARE @zz timestamp
            Select a=@a,b=@b,c=@c,d=@d,e=@e,f=@f,g=@g,h=@h,i=@i,j=@j,k=@k,l=@l,m=@m,n=@n,o=@o,p=@p,q=@q,r=@r,s=@s,t=@t,u=@u,v=@v,w=@w,x=@x,y=@y,z=@z,zz=@zz";

        public static string SelectAllNotNullTypes = @"
            DECLARE @a date='20181231'
            DECLARE @b time='10:11:12'
            DECLARE @c datetime2='20181231 10:11:12'
            DECLARE @d datetimeoffset='20181231 10:11:12+5:00'
            DECLARE @e bit=1
            DECLARE @f tinyint=1
            DECLARE @g smallint=1
            DECLARE @h int=1
            DECLARE @i bigint=1
            DECLARE @j real=1
            DECLARE @k float=1
            DECLARE @l money=1
            DECLARE @m smallmoney=1
            DECLARE @n smalldatetime='20181231'
            DECLARE @o datetime='20181231'
            DECLARE @p uniqueidentifier= cast('9e383328-69d7-4e73-8126-e25a1be94ae9' as uniqueidentifier)
            DECLARE @q BINARY(1)=CAST( 1 AS BINARY(1) )
            DECLARE @r varbinary(max)=CAST( replicate('123456789',1) AS varbinary(max) )
            DECLARE @s char(1)=CAST( 1 AS char(1) )
            DECLARE @t varchar(max)=CAST( replicate('123456789',1) AS varchar(max) )
            DECLARE @u nchar(1)=CAST( 1 AS nchar(1) )
            DECLARE @v nvarchar(max)=CAST( replicate('123456789',1) AS nvarchar(max) )
            DECLARE @w XML = '<Data><DepartmentID>{x}</DepartmentID></Data>'
            DECLARE @x decimal(4,1)=1
            DECLARE @y decimal(38,0)=power(10.0,18)
            DECLARE @z decimal(38,0)=power(10.0,28)
            DECLARE @zz timestamp=1
            Select a=@a,b=@b,c=@c,d=@d,e=@e,f=@f,g=@g,h=@h,i=@i,j=@j,k=@k,l=@l,m=@m,n=@n,o=@o,p=@p,q=@q,r=@r,s=@s,t=@t,u=@u,v=@v,w=@w,x=@x,y=@y,z=@z,zz=@zz
            UNION ALL Select a=null, b=null, c=null, d=null, e=null, f=null, g=null, h=null, i=null, j=null, k=null, l=null, m=null, n=null, o=null, p=null, q=null, r=null, s=null, t=null, u=null, v=null, w=null, x=null, y=null, z=null, zz=null 
            ";

        public static string SelectAllVariantTypes = @"
            DECLARE @a sql_variant=cast('20181231' as date)
            DECLARE @b sql_variant=cast('10:11:12' as time)
            DECLARE @c sql_variant=cast('20181231 10:11:12' as datetime2)
            DECLARE @d sql_variant=cast('20181231 10:11:12+5:00' as datetimeoffset)
            DECLARE @e sql_variant=cast(1 as bit)
            DECLARE @f sql_variant=cast(1 as tinyint)
            DECLARE @g sql_variant=cast(1 as smallint)
            DECLARE @h sql_variant=cast(1 as int)
            DECLARE @i sql_variant=cast(1 as bigint)
            DECLARE @j sql_variant=cast(1 as real)
            DECLARE @k sql_variant=cast(1 as float)
            DECLARE @l sql_variant=cast(1 as money)
            DECLARE @m sql_variant=cast(1 as smallmoney)
            DECLARE @n sql_variant=cast('20181231' as smalldatetime)
            DECLARE @o sql_variant=cast('20181231' as datetime)
            DECLARE @p sql_variant=cast('9e383328-69d7-4e73-8126-e25a1be94ae9' as uniqueidentifier)
            DECLARE @q sql_variant=cast( 1 AS BINARY(1) )
            --DECLARE @r sql_variant=cast( replicate('123456789',10) AS varbinary(max) )
            DECLARE @s sql_variant=cast(1 AS char(1) )
            --DECLARE @t sql_variant=cast(replicate('123456789',10) AS varchar(max) )
            DECLARE @u sql_variant=cast(1 AS nchar(1) )
            --DECLARE @v sql_variant=cast( replicate('123456789',10) AS nvarchar(max) )
            --DECLARE @w sql_variant=cast( '<Data><DepartmentID>{x}</DepartmentID></Data>' as XML)
            DECLARE @x sql_variant=cast(1 as decimal(4,1))
            DECLARE @y sql_variant=cast(power(10.0,18) as decimal(38,0))
            DECLARE @z sql_variant=cast(power(10.0,28) as decimal(38,0))
            Select a=@a,b=@b,c=@c,d=@d,e=@e,f=@f,g=@g,h=@h,i=@i,j=@j,k=@k,l=@l,m=@m,n=@n,o=@o,p=@p,q=@q,s=@s,u=@u,x=@x,y=@y,z=@z 
            UNION Select a=null, b=null, c=null, d=null, e=null, f=null, g=null, h=null, i=null, j=null, k=null, l=null, m=null, n=null, o=null, p=null, q=null, s=null, u=null, x=null, y=null, z=null 
            ";
    };
}
