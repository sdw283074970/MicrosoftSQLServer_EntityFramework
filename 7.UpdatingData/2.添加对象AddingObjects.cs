//Q: 那么如何在已有数据库中添加新的数据？
//A: 原理很简单，在Client中(在当前项目中指Main()方法)添加新的数据，改变追踪器CT就会将这些新的数据状态标记为新数据。然后调用SaveChanges()方法，EF就会
  //将新增的数据转化为SQL语句同步到数据库中。以PlutoContext为例，如果我们要在数据库中的Courses表中添加一个新条目，代码如下：

    static void Main(string[] args)
        {
            var context = new PlutoContext();   //获取PlutoContext的实例
            var course = new Course   //将新数据打包进course对象中作为Courses表中的新条目
            {
                Name = "New Course",
                Description = "New Description",
                FullPrice = 20f,
                Level = 1,
                Author = new Author { Id = 1, Name = "Mosh Hamedani" }
            };
            context.Courses.Add(course);    //将打包的对象添加进Courses表中，CT将标记course对象为Added状态
            context.SaveChanges();    //保存变更，EF生产对应的SQL语句，CT重置course对象状态为Unchanged
        }

  //按照步骤，数据添加基本完成。但是还有个小问题，即打开数据库Courses表，我们可以到看AuthorId这一栏为6而不是1，再打开Authors表我们可以看到多了一个
    //Id为6的重名作者。造成这情况的原因是我们在Author这地方新声明了一个Author的实例，CT把这个新实例也当作了新对象，CT并不知道数据库中已经有了一个
    //Id为1的重名作者。有两个方法可以解决这问题。第一种，将这个新的Author实例导入进DbContext即PlutoContext；第二种方法为使用AuthorId外键属性。

  //第一种方法在WPF项目中很有用，第二种方法更适合在Web应用中使用。我们分别讨论这两种方法。

  //第一种方法：使用已存在对象。在WPF项目中，我们的DbContext是长期存在的，只要窗口一打开，所有内容都加载至内存完毕，又等于说，只要窗口一关闭，所有内存
    //资源都将被释放。所以，只要一开始我们就取得目标实例(AuthorId等于1的对象)即可直接用在数据改变中。代码如下：

    static void Main(string[] args)
        {
            var context = new PlutoContext();
            var author = context.Authors.Single(a => a.Id == 1);    //获取Id==1的作者实例
            var course = new Course
            {
                Name = "New Course",
                Description = "New Description",
                FullPrice = 20f,
                Level = 1,
                Author = author   //直接将author实例对象作为新数据传入course中
            };
            context.Courses.Add(course);
            context.SaveChanges();
        }

  //需要注意的是，如果author对象不在context中，EF将尝试创建一个SQL查询从数据库中取得这个对象，这会造成一些性能惩罚。所以，仅在100%确认目标对象存在与
    //context中的时候使用这个方法。

  //第二种方法：使用外键属性。当新数据涉及到关系数据的时候，我们不用创建一个新的实例，直接使用其导航属性即可。上例代码如下啊：

    static void Main(string[] args)
        {
            var context = new PlutoContext();
            var course = new Course
            {
                Name = "New Course",
                Description = "New Description",
                FullPrice = 20f,
                Level = 1,
                AuthorId = 1    //直接使用导航属性AuthorId
            };
            context.Courses.Add(course);
            context.SaveChanges();
        }
  
  //这个方法适用在Web应用，是因为Web应用是短期存在的，在启动应用的时候没有对象存在内存中，Web应用向数据库发送请求，context等对象在建立后就释放了。
    //当然这个方法在WPF中也适用。

  //其实还有不常用的第三种方法。 假设我们有一个author对象但不存在于context中，但可以使用Attach()方法这个对象导入进context中与现有数据关联。
    //代码如下：

    static void Main(string[] args)
        {
            var author = new Author {Id = 1, Name = "Mosh Hamedani"};   //此对象不存在context中，但可以与context数据关联
            var context = new PlutoContext();
            context.Authors.Attach(author);   //使用Attach()方法将author与Authors关联，即在Auhtors表中添加author条目
            var course = new Course
            {
                Name = "New Course",
                Description = "New Description",
                FullPrice = 20f,
                Level = 1,
                Author = author   //author与context关联吼就可直接作为参数传入，CT不会将此对象视为新对象
            };
            context.Courses.Add(course);
            context.SaveChanges();
        }

  //第三种方法不推荐。原因是其专门使用Attach()这种功能性专一的方法。这种方法极可能在未来的EF版本中变名字导致现有代码不可使用。

//暂时想到这么多，最后更新2018/02/07
