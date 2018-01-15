//Q: LINQ语法可以实现哪些SQL的功能？
//A: 几乎所有常用的功能包括Restriction。本篇仍然以上篇的Queries项目为基础示例。
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

  //5.


