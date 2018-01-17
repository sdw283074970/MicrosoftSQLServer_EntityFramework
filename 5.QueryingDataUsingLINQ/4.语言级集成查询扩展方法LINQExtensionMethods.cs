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
              //.Select(c => c);   可省略，因为此时query的type就为Course
                

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
              //.Select(c => c);   可省略，因为此时query的type就为Course

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
              //此时query的type为匿名类'a new {int CourseLevel, string CourseName}

            foreach (var q in query)    //迭代输出结果
                Console.WriteLine("{0}, {1}", q.CourseLevel, q.CourseName);
        }

    //输出结果仍与上列一样，但是速度会快很多。
    //Select()方法的意思为返回目标值。如Select(c => c)即返回全部结果，query的类型就为Course；又如，Select(c => c.Description)即仅返回结果中
      //的Description列，query的类型为string。但是如果此时将Select()设为Select(c => c.Tags)的时候(Tags为多对多关系的另一个列表)，query的类型
      //就为IQueryable<T>，而T又为ICollection<Tag>，形成了一个表中表。由于是表中标，要输出IQueryable<T>类型中的值就需要迭代两次。代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses
                .Where(c => c.AuthorId == 1)
                .OrderByDescending(c => c.Level)
                .ThenBy(c => c.Name)
                .Select(c => c.Tags);   //选取结果中的Tags列，多对多关系中Tags本身也是表，所以query的类型为一个表中表，即IQueryable<T>
              //.SelectMany(c => c.Tags); 当选取结果中的对象为对多关系中的另一个表的时候可以用SelectMany()方法，则可以省去两次迭代正常输出

            foreach (var q in query)    //迭代输出结果中的信息，需要两次迭代
            {
                foreach (var t in q)    //再次迭代获取一个Course条目中对应Tags表中的信息
                    Console.WriteLine("TagName:{0}, TagId:{1}", t.Name, t.Id);
            }
          //foreach (var q in query)
          //    Console.WriteLine("TagName:{0}, TagId:{1}", q.Name, q.Id);
        }

    //用Select()方法和SelectMany()方法的输出结果都一样。

  //4.设置操作符SET OPERATORS。设置操作符有四种，分别是Distinct(剔除结果中重复的条目)、Intersect、Union和Expect。代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses
                .Where(c => c.AuthorId == 1)
                .OrderByDescending(c => c.Level)
                .ThenBy(c => c.Name)
                .SelectMany(c => c.Tags)
                .Distinct();  //在结尾处调用Distinct()方法来剔除重复的结果

            foreach (var q in query)
                Console.WriteLine("TagName:{0}, TagId:{1}", q.Name, q.Id);
        }

  //5.分组Grouping。在LINQ中我们用Group方法按照某个标准分类来分解表。可以理解为GroupBy()方法的返回类型为ICollection<Group<T>>，即列表中的列表。
    //其中，Group<T>中的T类比原类多一个Key字段。同样，假设项目需求为将Courses表按照课程难度等级分类，并按难度类别输出查询结果。
    //使用扩展方法的代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();
            var query = context.Courses.GroupBy(c => c.Level);    //调用GroupBy()方法

            foreach (var q in query)    //迭代输出结果，此处q为按条件被分解的表
            {
                Console.WriteLine("Level: {0}", q.Key);
                foreach (var g in q)    //迭代输出结果，此处g为被分解表中的条目
                {
                    Console.WriteLine("        {0}", g.Name);
                }
            }
        }

    //输出结果为：

      //Level: 1
      //        C# Basics
      //        Programming for Complete Beginners
      //        A 16 Hour C# Course with Visual Studio 2013
      //        Learn JavaScript Through Visual Studio 2013
      //Level: 2
      //        C# Intermediate
      //        Javascript: Understanding the Weird Parts
      //        Learn and Understand AngularJS
      //        Learn and Understand NodeJS
      //Level: 3
      //        C# Advanced

    //下面考虑累计函数的情况。在LINQ中累计函数并没有与GroupBy()方法绑定，所以其实考虑应用累计函数也不涉及到GroupBy()的使用，直接对结果进行操作即可。
      //同样假设项目去要查询每个Author出的课程总值，这里要求输出Author的Name而不仅仅是AuthorId，则代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();
            var query = context.Courses
                .GroupBy(c => c.AuthorId);
          
            foreach (var q in query)    //迭代输出
            {
                var authorQuery = context.Authors   //我们只有AuthorId，要输出与AuthorId对应的AuthorName，还需要查询获取包括AuthorName的条目
                    .Where(a => a.Id == q.Key);
                foreach (var a in authorQuery)    //第二次迭代输出。authorQuery包括了AuthorName的条目，query包括了价格，即可完成输出
                    Console.WriteLine("AuthorName: {0}, TotalPrice: {1}", a.Name, q.Sum(c => c.FullPrice));
            }
        }

    //输出结果：
      //AuthorName: Mosh Hamedani, TotalPrice: 167
      //AuthorName: Anthony Alicea, TotalPrice: 397
      //AuthorName: Eric Wise, TotalPrice: 45
      //AuthorName: Tom Owsiak, TotalPrice: 170

    //此外，我们注意到AuthorId是作为一个外键，一个导航来连接Courses表和Authors表，即Courses.AuthorId永远等于Authors.Id.可以利用这一点来直接通过
      //导航来对外表等值的列进行分组。即在此例中，可以对Courses表按Author.Name进行分解打碎，其逻辑为Courses.AuthorId==Authors.Id==Authors.Name。
      //这样我们就可以省略对Authors.Name的查询，直接通过Course.Author.Name获取AuthorName。其代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses
                .GroupBy(c => c.Author.Name);   //之间在Courses表中按Authors表中的Name进行分类打碎

            foreach (var q in query)    //迭代输出，query.Key即为分组的种类，即AuthorName
            {
                Console.WriteLine("AuthorName: {0}, TotalPrice: {1}", q.Key, q.Sum(c => c.FullPrice));
            }
        }//输出结果与上相同

  //5.结合Join。再提一下，LINQ中有三种JOIN方式分别为INNER JOIN、GROUP JOIN和CROSS JOIN。LINQ中的InnerJoin和CrossJoin与SQL中的很像，但在LINQ中
    //使用GroupJoin有点像在SQL中使用LEFTJOIN。我们再次分开详解。

    //1）LINQ中的INNERJOIN。在SQL中，INNER JOIN为两个表的交集，通常用在两个没有关系的表中，在LINQ中也一样。假设Courses表和Authors表没有外键联通的
      //关系，项目需求为查询返回一张课程名和作者名在一起的表，格式为“课名 - 作者名”，我们就要用到INNER JOIN。代码和结果如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses   //选择表1
                .Join(context.Authors,    //选择表2
                    c => c.AuthorId, //选择表1的列
                    a => a.Id,    //选择表2的列，此处认为表1列的值等于表2列的值
                    (Course, Author) => new { CourseName = Course.Name, AuthorName = Author.Name });//只要CourseName和AuthorName，所以投射

            foreach (var q in query)    //迭代输出结果
            {
                Console.WriteLine("{0} - {1}", q.CourseName, q.AuthorName);
            }
        }

      //Join()方法要求传递四个参数，分别是Join(Entity, Func<Course, T>, Func<Author, Int>, Func<Course, Author, TResult>)。
      //输出结果为：

        //C# Basics - Mosh Hamedani
        //C# Intermediate - Mosh Hamedani
        //C# Advanced - Mosh Hamedani
        //Javascript: Understanding the Weird Parts - Anthony Alicea
        //Learn and Understand AngularJS - Anthony Alicea
        //Learn and Understand NodeJS - Anthony Alicea
        //Programming for Complete Beginners - Eric Wise
        //A 16 Hour C# Course with Visual Studio 2013 - Tom Owsiak
        //Learn JavaScript Through Visual Studio 2013 - Tom Owsiak

    //











