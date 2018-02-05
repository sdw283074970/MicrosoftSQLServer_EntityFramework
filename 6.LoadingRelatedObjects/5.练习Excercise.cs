//项目数据库简介：Download Section 7 - Exercise Files.zip attached to this lecture and open the solution in Visual Studio. Then, 
//go to Tools > Package Manager > Package Manager Console and run Update-Database. This will generate a new database called Vidzy_Queries 
//populated with some data. 重新设置connectionString，Video类与Genre类为多对一关系，即一个Video只有一种Genre，然而一种Genre可以对应多个Video，
//并且这两个类的导航属性都没有用Virtual修饰，懒惰加载默认为开。

//项目需求：
  //1.不使用任何加载方法迭代输出Videos表中的所有Video名称和Genre，观察输出结果；
  //2.使用懒惰加载迭代输出Videos表中的所有Video名称和Genre，使用SQL Sever Profiler观察N+1问题；
  //3.使用贪婪加载迭代输出Videos表中的所有Video名称和Genre，使用SQL Sever Profiler观察N+1问题是否得到解决；
  //4.使用显示加载迭代输出Videos表中的所有Video名称和Genre。
  //5.要求所有输出结果如下：
  
    //Name: The Godfather, Genre: Drama
    //Name: The Shawshank Redemption, Genre: Drama
    //Name: Schindler's List, Genre: Drama
    //Name: The Hangover, Genre: Comedy
    //Name: Anchorman, Genre: Comedy
    //Name: Die Hard, Genre: Action
    //Name: The Dark Knight, Genre: Action
    //Name: Terminator 2: Judgment Day, Genre: Action

//1.在不使用任何加载方法的情况输出结果。代码如下：

        static void Main(string[] args)
        {
            var context = new VidzyContext();
            var query = context.Videos.ToList();    //直接进行查询，返回Video条目

            foreach (var q in query)  //迭代输出结果
                Console.WriteLine("Name: {0}, Genre: {1}", q.Name, q.Genre.Name);
        }//结果为NULLREFERENCEEXCEPTION，抛出空引用异常。原因是无法获得导航属性Genre的加载。使用懒惰加载可修复此问题。
        
//2.在所有导航属性前插入Virtual修饰符，即启用懒惰加载。代码如下：

        static void Main(string[] args)
        {
            var context = new VidzyContext();
            var query = context.Videos.ToList();

            foreach (var q in query)
                Console.WriteLine("Name: {0}, Genre: {1}", q.Name, q.Genre.Name);
        }//输出正常，在 SQL Sever Profiler 中观察到N+1问题
        
//3.插入Include()方法使用贪婪加载，代码如下：

        static void Main(string[] args)
        {
            var context = new VidzyContext();
            var query = context.Videos.Include(v => v.Genre).ToList();    //插入Include()方法，使用第二个重载加载Genre的所有属性

            foreach (var q in query)
                Console.WriteLine("Name: {0}, Genre: {1}", q.Name, q.Genre.Name);
        }//输出正常，在 SQL Sever Profiler 中没有观察到N+1问题

//4.使用显示加载。直接加载Genres，代码如下：

        static void Main(string[] args)
        {
            var context = new VidzyContext();
            context.Genres.Load();    //直接加载Genres表
            var query = context.Videos.ToList();

            foreach (var q in query)
                Console.WriteLine("Name: {0}, Genre: {1}", q.Name, q.Genre.Name);
        }//输出正常，在 SQL Sever Profiler 中没有观察到N+1问题
      
    //通过条件加载Genres，代码如下：

        static void Main(string[] args)
        {
            var context = new VidzyContext();
            var genres = context.Genres.Select(g =>g.Id);   //取得IEnumerable<Genre>类型
            context.Videos.Where(v => genres.Contains(v.GenreId)).Load();   //进行条件显示加载
            var query = context.Videos.Include(v => v.Genre).ToList();

            foreach (var q in query)
                Console.WriteLine("Name: {0}, Genre: {1}", q.Name, q.Genre.Name);
        }//输出正常，在 SQL Sever Profiler 中没有观察到N+1问题
        
//项目完成。最后更新2018/02/04
