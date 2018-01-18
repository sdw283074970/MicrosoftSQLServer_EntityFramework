//Q: 之前说LINQ语法是LINQ扩展方法的子集，在扩展方法篇章中能找到LINQ语法的扩展方法版本。有没有一些LINQ语法不支持的扩展方法？
//A: 有。以下分别列举。
  //1.分割Partitioning。我们可以用Skip()&Take()方法跳过一定数量，查找并返回又一定数量的条目。使用Skip()方法前要用OrderBy()方法排序。假设项目需求为
    //返回并显示从第5、6条记录。我们可以有以下代码：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses
                .OrderBy(c => c.AuthorId)
                .Skip(4).Take(2);

            foreach (var q in query)
                Console.WriteLine(q.Name);
        }

    //输出结果为：

      //Learn and Understand AngularJS
      //Learn and Understand NodeJS 

  //2.元素操作符Element Operators。指一套相似的方法：First()/FirstOrDefault()方法、Last()/LastOrDefault()方法、Single()/SingleOrDefault()方法。
    //所有方法都要求对象已排序(用OrderBy()方法)，这三套方法分别指返回第一个/最后一个/单个对象，如果返回的是NULL值，则会抛出异常，这种情况应采用
    //XXXOrDefault()方法。
    
    //注意，使用Single()一套方法时，如果返回的是多个对象，则一样会抛出异常。
    //再注意，Last()一套方法不能在SQL Sever中使用，这是为别的类型的数据库设计的方法。

  //3.定量Quantifying。一些很简单的定量方法，如All()方法和Any()方法。他们的参数都为Func<T, bool>，即判断所有对象(ALL)或至少一个(ANY)满足条件，
    //返回类型为布尔值。

  //4.累计Aggregating。即之前经常说的累计函数。包括Count()方法、Max()方法、Min()方法和Sum()方法。一目了然不做赘述。

//暂时想到这么多，最后更新2018/01/17
