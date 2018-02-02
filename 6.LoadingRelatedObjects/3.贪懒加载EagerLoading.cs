//Q: 什么是贪婪加载？
//A: 贪婪加载与懒惰加载相反，即通过在最开始就将所有关系型数据全部加载来避免像贪婪加载那样的多次额外加载。

//Q: 如何使用贪婪加载？
//A: 在任何触发查询的方法前插入Include()方法即可。如，课程这个表中有导航属性Author，在返回Courses表的时候懒惰加载并不会加载导航属性Author。若要通过
  //courses迭代输出Author将会造成N+1问题。现可在如ToList()方法之前插入Include()方法，在EF将本句转为SQL查询之前就指定要加载的所有条目，即贪婪加载。
  //代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var courses = context.Courses.Include("Author").ToList();   //贪婪加载所有Author导向的条目，此处执行一次查询
          
            foreach (var course in courses)   //所有数据现在储存在内存中，直接从内存迭代，此处不执行额外的查询
              Console.WriteLine("{0} by {1}", course.Name, course.Author.Name);
        }
  
  //当然，在Include()方法传入string参数是个很糟糕的做法，如果Author改名了，这里不会抛出异常，也不会告诉我们加载失败。程序其他地方照样运行。正确的
    //做法是应该避免使用传入string参数这一重载，应该改用传如委托参数的重载。改进代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var courses = context.Courses.Include(c => c.Author).ToList();   //使用匿名表达式传入参数
          
            foreach (var course in courses)
              Console.WriteLine("{0} by {1}", course.Name, course.Author.Name);
        }

  //这样Include()方法中的参数就能用Rename Reactor来更名了。

//Q: 贪婪加载是否可以加载多重关系的数据？
//A: 可以。如，Course有Author的导航属性，Author有Address的导航属性，在我们查询Course的时候想一并知道Author的Address属性，就可以如下操作：

        //单一属性多重查询
        context.Course.Include(c => c.Author.Address);

        //多重属性(集合)多重查询
        context.Course.Include(c => c.Tags.Select())
