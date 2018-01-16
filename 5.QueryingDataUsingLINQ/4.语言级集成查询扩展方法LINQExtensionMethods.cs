//Q: 是否也有一套与LINQ语法对应功能的扩展方法？
//A: 当然是，而且更准确的说法为LINQ语法对应扩展方法的功能。因为LINQ语法本身调用的也是扩展方法，是扩展方法的一个子集。我们将使用扩展方法来还原所有在
  //LINQ语法中出现过的例子。
  //1.限制Restriction。仍然用关键词Where，但这里是实打实像FluentAPI那样调用扩展方法Where()。如项目要求找出所有AuthorId为1且级别为LEVEL1的课程，
    //使用扩展方法的代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();
            //Where()方法作用为基于Predict过滤出一个序列的值。
            //Predict在这里指Expression<Func<Course, bool>>类，传入Course类，返回布尔值作为参数，为真保留，为否则过滤
            var query = context.Courses
                .Where(c => c.Level == 1 && c.AuthorId == 1);  //调用Where()方法，内传入条件，返回布尔值参数
              //.Select(c => c);   可省略
                

            foreach (var q in query)    //迭代输出
                Console.WriteLine(q.Name);
        }

    //输出结果为： C# Basics

  //2.排序Ordering。扩展方法支持像FluentAPI那样链式调用，关键词OrderBy。如项目要求找出所有AuthorId为1的课程，并按Level降序排列，再按名字升序排列。
    //使用扩展方法代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses
                .Where(c => c.AuthorId == 1)  //过滤选择AuthorId等于1的条目
                .OrderByDescending(c => c.Level)    //将这些条目按Level降序排列
                .ThenBy(c => c.Name);    //再将这些条目按Name升序排列
              //.Select(c => c);   可省略

            foreach (var q in query)    //迭代输出结果
                Console.WriteLine("{0}, {1}", q.Level, q.Name);
        }

    //输出结果为：

      //3, C# Advanced
      //2, C# Intermediate
      //1, C# Basics

  //3.投射Projection。为了优化效率，在上例中我们只要求输出课程等级和课程名字。使用扩展方法代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses
                .Where(c => c.AuthorId == 1)
                .OrderByDescending(c => c.Level)
                .ThenBy(c => c.Name)
                .Select(c => new { CourseLevel = c.Level, CourseName = c.Name});  //将结果投射到新的匿名类，抛弃无用属性以优化效率

            foreach (var q in query)    //迭代输出结果
                Console.WriteLine("{0}, {1}", q.CourseLevel, q.CourseName);
        }

    //输出结果仍与上列一样，但是速度会快很多。

  //4.






