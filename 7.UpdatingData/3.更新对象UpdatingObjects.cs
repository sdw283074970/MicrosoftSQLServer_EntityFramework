//Q: 如何更新数据？
//A: 首先将需要更新的数据对象导入进内存中，即建立一个实例，然后就可以为所欲为更新对象，最后调用SaveChanges()方法可以。仍然以PlutoContext为例，
  //代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();   //将数据对象集合导入内存
          
            var course = context.Courses.Find(1);   //使用Find()方法在Courses表中查找主键为1的条目，此时此条目状态为Unchanged
            course.Name =  "IDK";   //CT追踪到变化，将course对象以及Name状态改为Modified
            course.AuthorId = 2;    //同样CT将course对象中的AuthorId对象状态改为Modified
          
            context.SaveChanges();    //保存更改，EF生成对应的SQL语句同步数据库
        }

  //需要注意的是，此处AuthorId为导航属性。与上节一样，这里也可以使用在内存中已存在的其他对象。代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();
          
            var author = context.Authors.Find(2);   //将Authors表中主键为2的条目加载进内存
            var course = context.Courses.Find(1);
            course.Name =  "IDK";
            course.Author = author;    //将内存中的author对象赋予course对象中的Author属性中，与使用导航属性等价
          
            context.SaveChanges();
        }

//暂时想到这么多，最后更新2018/02/07
