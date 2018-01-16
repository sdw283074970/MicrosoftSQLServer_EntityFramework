//Q: LINQ语法可以实现哪些SQL的功能？
//A: 几乎所有常用的查询功能包括过滤、排序、投射、分组、结合等。本篇仍然以上篇的Queries项目为基础示例。
  //1.限制Restriction。即SQL语句中的WHERE语句。如项目要求找出所有AuthorId为1且级别为LEVEL1的课程，代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from c in context.Courses //LINQ语法总是以from起头，select结尾
                        where c.Level == 1 && c.Author.Id == 1    //where为限制关键词，等于SQL语句中的WHERE，&&等于SQL语句中的AND
                        select c;
        }
  
  //2.排序Ordering。即SQL语句中的ORDERBY语句。如项目要求找出所有AuthorId为1的课程，并按Level降序排列，再按名字升序排列。代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from c in context.Courses 
                        where c.Author.Id == 1
                        orderby c.Level descending, c.Name  //以多个元素为基准进行排列。默认为升序排列，降序需加上关键词descending
                        select c;
        }

  //3.投射Projection。在一些生产情况中，为了优化，我们并不想将查询到的所有结果返回，只想返回有用的属性。如上例，Course类有很多属性，经过筛选、
    //排序后，我们查询到了几个对象，但是这些对象中的属性又只有Name和Author.Name是有用的，其他的比如Level、Description等属性没用，为了优化程序性能，
    //我们通常只返回查询结果中有用的属性，这个时候就需要投射。那么应该投射到哪里？一个方法是新建一个只有Name和Author的类然后将结果投射进去，另一个方法
    //就是直接实例化一个匿名类/对象。代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from c in context.Courses 
                        where c.Author.Id == 1
                        orderby c.Level descending, c.Name
                        select new { Name = c.Name, Author = c.Author.Id };   //将结果投射至一个属性只有Name和Author的匿名类
        }

    //投射后的区别为，2中的qurey储存了Course类所有的属性及其值(包括Level、Description)，而3中的Qurey只有Name和Author两个属性和值，达到了优化目的。

  //4.分组Grouping。注意此处的Group与SQL中的不同。在SQL中GROUPBY是结合合计函数(Aggragate Function，如SUM、COUNT、MAX)来对一个或多个列进行分组。
    //而LINQ中的groupby的功能是，将一个列表打碎后，以某一列为基准，将此列值相同的条目组合起来，形成多个表(组)，并以这个列值作为每个表的Key值加以区分。
    //如一个表中有四个条目，其中两个条目的Level列都为1，另外两个分别为2和3。以Level为基准使用groupby，打碎原表后将相同Level的课程重新结合在一起，即
    //现在有三个表，分别为Level为1的表、Level为2的表和Level为3的表，他们的Key值分别为1、2、3，因为Level有1、2、3三种。代码和查询结果如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from c in context.Courses
                        group c by c.Level    //以Level列为基准经行分组
                        into g    //g为在该查询的临时声明变量
                        select g;

            foreach (var group in qurey)    //对组进行迭代输出，因为Level有3种所有输出有3个组
            {
                Console.WriteLine(group.Key);   //输出key值

                foreach (var c in group)    //输出每一组的成员
                    Console.WriteLine("\t{0}", c.Name);   //输出成员Name
            }
        }
  
    //输出结果如下：

        //1   组的Key值，值与Level列值相同
        //        C# Basics   所有等级为Level1的课程名
        //        Programming for Complete Beginners
        //        A 16 Hour C# Course with Visual Studio 2013
        //        Learn JavaScript Through Visual Studio 2013
        //2
        //        C# Intermediate   所有等级为Level2的课程名
        //        Javascript: Understanding the Weird Parts
        //        Learn and Understand AngularJS
        //        Learn and Understand NodeJS
        //3
        //        C# Advanced   所有等级为Level3的课程名

    //以上例子都与累计函数无关，即没有累计函数也可使用groupby。但如果我们要像SQL一样，查询每个Author出的课程总值为多少的表呢？这就需要用到累计函数，
      //用法也很简单，代码和结果如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from c in context.Courses
                        group c by c.Author.Id    //以Author的Id列为基准分组，相同的Id分为一组
                        into g
                        select g;

            foreach (var group in qurey)
            {
                //分组后，可以针对条目的属性调用任意方法来达到我们的查询目的，如累加同一组的FullPrice列值
                //group类中还有其他累计函数，如Max()、Count()等，按需自取
                Console.WriteLine("Author Id: {0}, Total Price: {1}", group.Key, group.Sum(c => c.FullPrice));    //调用group的Sum()函数
            }
        }

      //输出结果为：

        //Author Id: 1, Total Price: 167  Id为1的作者发起的课程总价为167
        //Author Id: 2, Total Price: 397  Id为2的作者发起的课程总价为397
        //Author Id: 3, Total Price: 45 Id为3的作者发起的课程总价为45
        //Author Id: 4, Total Price: 170  Id为4的作者发起的课程总价为170

  //5.结合Join。在SQL中我们有三种Join，分别是INNER JOIN、GROUP JOIN和CROSS JOIN。LINQ中的InnerJoin和CrossJoin与SQL中的很像，但在LINQ中使用
    //GroupJoin有点像在SQL中使用LEFTJOIN。我们分开详解。
    
    //1）LINQ中的INNERJOIN。在SQL中，INNER JOIN为两个表的交集，在LINQ中也一样。假设项目需求为查询返回一张课程名和作者名在一起的表，
      //格式为“课名 - 作者名”，我们就要用到INNER JOIN。代码和结果如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from c in context.Courses
                        join a in context.Authors on c.AuthorId equals a.Id   //选择两个表中所有AuthorId等于Id的条目为结果
                        select new { AuthorName = a.Name, CourseName = c.Name };    //优化程序，将结果有用的部分投射到匿名类中

            foreach (var x in qurey)  //迭代输出结果
                Console.WriteLine("{0} - {1}", x.CourseName, x.AuthorName);
        }
      
      //输出结果为:

        //C# Basics - Mosh Hamedani
        //C# Intermediate - Mosh Hamedani
        //C# Advanced - Mosh Hamedani
        //Javascript: Understanding the Weird Parts - Anthony Alicea
        //Learn and Understand AngularJS - Anthony Alicea
        //Learn and Understand NodeJS - Anthony Alicea
        //Programming for Complete Beginners - Eric Wise
        //A 16 Hour C# Course with Visual Studio 2013 - Tom Owsiak
        //Learn JavaScript Through Visual Studio 2013 - Tom Owsiak

      //以上为普通情况。如果两个表中有导向属性，即外键关系，则可不用关键词，直接输出结果。如Courses表和Authors表本来就有导航属性，即Courses表中
        //的AuthorId字段直接指向了Authors表中的Id字段，则可直接输出以上需求，代码如下;

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from c in context.Courses
                        //因为AuthorId为从Courses表指向Author表的外键的关系，我们可以直接通过调用Courses表来取得Authors表中的引用
                        select new { AuthorName = c.Author.Name, CourseName = c.Name };    //优化程序，将结果有用的部分投射到匿名类中

            foreach (var x in qurey)  //迭代输出结果
                Console.WriteLine("{0} - {1}", x.CourseName, x.AuthorName);
        }//输出结果与上面是一样的

    //2）LINQ中的GROUP JOIN。相当于SQL中的LEFT JOIN，经常会在实际生产中配合累计函数查询结果。如，我们要查询每个作者发起的课程总价值，结果中要返回
      //作者的名字(不是GROUPBY例子中的Id)和总价格，就要用到GROUP JOIN。其中，由于Authors表和Courses表为一对多的关系，Authors表应该为左表，Courses
      //为右表，代码和结果如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from a in context.Authors
                        join c in context.Courses on a.Id equals c.AuthorId
                        into g    //之前是标准的INNERJOIN，加上into后就变成GROUPJOIN，g为左表Authors的完全集
                        select new { AuthorName = a.Name, TotalPrice = g.Sum(c => c.FullPrice) };

            foreach (var x in qurey)
                Console.WriteLine("{0} - {1}", x.AuthorName, x.TotalPrice);
        }

      //输出结果为(与GROUPBY例子结果相同)：

        //Author Id: 1, Total Price: 167  Id为1的作者发起的课程总价为167
        //Author Id: 2, Total Price: 397  Id为2的作者发起的课程总价为397
        //Author Id: 3, Total Price: 45 Id为3的作者发起的课程总价为45
        //Author Id: 4, Total Price: 170  Id为4的作者发起的课程总价为170

    //3）LINQ中的CROSS JOIN。与SQL中的CROSS JOIN相同，返回左表条目*右边条目的结果。如过我们将Authors表和Courses表交叉结合，将产生一些没有意义的
      //条目，如一个写C#的作者写了一本JS的书。在实际生产中，交叉结合的结果需要进一步筛选才能使用。如，项目需求为Authors表与Courses表交叉集合，返回
      //一切作者和书名的结合可能，代码和输出结果如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var qurey = from a in context.Authors
                        from c in context.Courses   //CROSSJOIN直接两个from关键词走起，然后投射即可
                        select new { AuthorName = a.Name, CourseName = c.Name };

            foreach (var x in qurey)    //迭代输出结果
                Console.WriteLine("{0} - {1}", x.AuthorName, x.CourseName);
        }

      //输出结果为Authors*Courses的一切结合可能：

        //Mosh Hamedani - Learn and Understand AngularJS
        //Mosh Hamedani - Learn and Understand NodeJS
        //Mosh Hamedani - Programming for Complete Beginners
        //Mosh Hamedani - A 16 Hour C# Course with Visual Studio 2013
        //Mosh Hamedani - Learn JavaScript Through Visual Studio 2013
        //Anthony Alicea - C# Basics
        //Anthony Alicea - C# Intermediate
        //Anthony Alicea - C# Advanced
        //Anthony Alicea - Javascript: Understanding the Weird Parts
        //Anthony Alicea - Learn and Understand AngularJS
        //Anthony Alicea - Learn and Understand NodeJS
        //Anthony Alicea - Programming for Complete Beginners
        //Anthony Alicea - A 16 Hour C# Course with Visual Studio 2013
        //Anthony Alicea - Learn JavaScript Through Visual Studio 2013
        //Eric Wise - C# Basics
        //Eric Wise - C# Intermediate
        //Eric Wise - C# Advanced
        //Eric Wise - Javascript: Understanding the Weird Parts
        //Eric Wise - Learn and Understand AngularJS
        //Eric Wise - Learn and Understand NodeJS
        //Eric Wise - Programming for Complete Beginners
        //Eric Wise - A 16 Hour C# Course with Visual Studio 2013
        //Eric Wise - Learn JavaScript Through Visual Studio 2013
        //Tom Owsiak - C# Basics
        //Tom Owsiak - C# Intermediate
        //Tom Owsiak - C# Advanced
        //Tom Owsiak - Javascript: Understanding the Weird Parts
        //Tom Owsiak - Learn and Understand AngularJS
        //Tom Owsiak - Learn and Understand NodeJS
        //Tom Owsiak - Programming for Complete Beginners
        //Tom Owsiak - A 16 Hour C# Course with Visual Studio 2013
        //Tom Owsiak - Learn JavaScript Through Visual Studio 2013
        //John Smith - C# Basics
        //John Smith - C# Intermediate
        //John Smith - C# Advanced
        //John Smith - Javascript: Understanding the Weird Parts
        //John Smith - Learn and Understand AngularJS
        //John Smith - Learn and Understand NodeJS
        //John Smith - Programming for Complete Beginners
        //John Smith - A 16 Hour C# Course with Visual Studio 2013
        //John Smith - Learn JavaScript Through Visual Studio 2013

//暂时想到这么多，最后更新2018/01/15
