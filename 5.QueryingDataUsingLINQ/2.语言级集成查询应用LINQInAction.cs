//Q: 这次的应用例子仍然是Pluto么？
//A: 框架上是，但是既然要查询就得有被查询的内容，这次的例子仍然沿用Pluto数据库的设计，但是额外的添加了一些种子数据。种子数据之前说过储存在Migrations
  //文件夹下的配置文件Configuration.cs中的Seed()方法中，除此之外与之前的Pluto并无区别。Configuration类源代码如下：

    internal sealed class Configuration : DbMigrationsConfiguration<PlutoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PlutoContext context)
        {
            #region Add Tags
            var tags = new Dictionary<string, Tag>
            {
                {"c#", new Tag {Id = 1, Name = "c#"}},
                {"angularjs", new Tag {Id = 2, Name = "angularjs"}},
                {"javascript", new Tag {Id = 3, Name = "javascript"}},
                {"nodejs", new Tag {Id = 4, Name = "nodejs"}},
                {"oop", new Tag {Id = 5, Name = "oop"}},
                {"linq", new Tag {Id = 6, Name = "linq"}},
            };

            foreach (var tag in tags.Values)
                context.Tags.AddOrUpdate(t => t.Id, tag);
            #endregion

            #region Add Authors
            var authors = new List<Author>
            {
                new Author {Id = 1, Name = "Mosh Hamedani"},
                new Author {Id = 2, Name = "Anthony Alicea"},
                new Author {Id = 3, Name = "Eric Wise"},
                new Author {Id = 4, Name = "Tom Owsiak"},
                new Author {Id = 5, Name = "John Smith"}
            };

            foreach (var author in authors)
                context.Authors.AddOrUpdate(a => a.Id, author);
            #endregion

            #region Add Courses
            var courses = new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Name = "C# Basics",
                    AuthorId = 1,
                    FullPrice = 49,
                    Description = "Description for C# Basics",
                    Level = 1,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"]
                    }
                },
                new Course
                {
                    Id = 2,
                    Name = "C# Intermediate",
                    AuthorId = 1,
                    FullPrice = 49,
                    Description = "Description for C# Intermediate",
                    Level = 2,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"],
                        tags["oop"]
                    }
                },
                new Course
                {
                    Id = 3,
                    Name = "C# Advanced",
                    AuthorId = 1,
                    FullPrice = 69,
                    Description = "Description for C# Advanced",
                    Level = 3,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"]
                    }
                },
                new Course
                {
                    Id = 4,
                    Name = "Javascript: Understanding the Weird Parts",
                    AuthorId = 2,
                    FullPrice = 149,
                    Description = "Description for Javascript",
                    Level = 2,
                    Tags = new Collection<Tag>()
                    {
                        tags["javascript"]
                    }
                },
                new Course
                {
                    Id = 5,
                    Name = "Learn and Understand AngularJS",
                    AuthorId = 2,
                    FullPrice = 99,
                    Description = "Description for AngularJS",
                    Level = 2,
                    Tags = new Collection<Tag>()
                    {
                        tags["angularjs"]
                    }
                },
                new Course
                {
                    Id = 6,
                    Name = "Learn and Understand NodeJS",
                    AuthorId = 2,
                    FullPrice = 149,
                    Description = "Description for NodeJS",
                    Level = 2,
                    Tags = new Collection<Tag>()
                    {
                        tags["nodejs"]
                    }
                },
                new Course
                {
                    Id = 7,
                    Name = "Programming for Complete Beginners",
                    AuthorId = 3,
                    FullPrice = 45,
                    Description = "Description for Programming for Beginners",
                    Level = 1,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"]
                    }
                },
                new Course
                {
                    Id = 8,
                    Name = "A 16 Hour C# Course with Visual Studio 2013",
                    AuthorId = 4,
                    FullPrice = 150,
                    Description = "Description 16 Hour Course",
                    Level = 1,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"]
                    }
                },
                new Course
                {
                    Id = 9,
                    Name = "Learn JavaScript Through Visual Studio 2013",
                    AuthorId = 4,
                    FullPrice = 20,
                    Description = "Description Learn Javascript",
                    Level = 1,
                    Tags = new Collection<Tag>()
                    {
                        tags["javascript"]
                    }
                }
            };

            foreach (var course in courses)
                context.Courses.AddOrUpdate(c => c.Id, course);
            #endregion
        }
    }

  //简而言之，在Tags表、Authors表和Courses表中添加了种子数据。
  //接着更改App.config中的connectionStrings信息(如有必要)，建立迁移文件夹，初始化迁移并同步建立数据库。本次应用的初始工作就算完成。
  
//Q: 如何查询数据？
//A: 要查询数据，首先要取得Domain类也就是DbContext类，在这里就是PlutoDbContext类的实例。在Program类中的Main()方法中实例化PlutoDbContext：

    class Program
    {
        static void Main(string[] args)
        {
            var context = new PlutoContext();
        }
    }

  //在LINQ中我们可以使用两种方法查询数据。一种是LINQ语法(LINQ Syntax)，另一种是LINQ扩展方法(LINQ Extention Methods)。假设项目需求要求查询出所有
    //C#的课程并返回值，这两种方法都能达到目的。首先来看LINQ语法。

  //LINQ语法很像SQL语法，使用LINQ语法查询C#课程代码如下：

    class Program
    {
        static void Main(string[] args)
        {
            var context = new PlutoContext();

            //LINQ syntax
            var qurey = from c in context.Courses   //c是在查询中声明的临时变量，指查询结果(c执行到这里为全体)，context.Courses为数据源
                        where c.Name.Contains("c#")   //where为条件关键词，筛选名字中带c#的对象(c执行到这里就只剩名字中带c#的对象)
                        orderby c.Name    //以名字为标准排序
                        select c;   //LINQ语法最后都以select结尾，选取剩下的对象返回给变量qurey，此时qurey为一个列表

            foreach (var course in qurey)   //迭代输出
                Console.WriteLine(course.Name);
        }
    }

  //扩展方法就跟SQL语法不像了，为标准的C#语法(FluentAPI）。使用扩展方法查询c#课程代码如下：

    class Program
    {
        static void Main(string[] args)
        {
            var context = new PlutoContext();

            //Extension methods
            var courses = context.Courses   //context.Courses为数据源
                .Where(c => c.Name.Contains("c#"))    //Where方法，作用为基于条件判断筛选一系列值，这里的条件为名字中含有c#
                .OrderBy(c => c.Name);    //排序

            foreach (var course in courses)   //迭代输出结果
                Console.WriteLine(course.Name);
        }
    }

  //之所以叫扩展方法是因为像Where、OrderBy都不是Courses中的方法，而是扩展上去的。
  //扩展方法详见《CS_Advanced_LearningNote/5.扩展方法Extension Methods.cs》

//Q: 那两种方法用哪一种比较好？
//A: 两种都学，但主要用扩展方法。LINQ语法写起来简单，但也是扩展方法的子集，没有扩展方法也就没有类SQL语法。

//暂时想到这么多，最后更新2018/01/13
