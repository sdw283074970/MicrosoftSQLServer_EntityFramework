//Q: 什么是种子化数据库？
//A: 种子化(Seed)是在Migration文件夹下的Configuration.cs中的方法，即是Configuration类中的一个方法，而这个类的命名空间为[ProjectName].Migrations.
  //每一次在PM中执行update-database命令后，该方法都会执行一次，可以看作是数据库的初始化，为数据库赋予/更新一些初始内容。

//Q: Configuration是一个什么样的类？
//A: Configuration是用来储存迁移类设置的类，其结构如下：

    internal sealed class Configuration : DbMigrationsConfiguration<CodeFirstExistingDb.PlutoContext>
    {
        public Configuration()    //构造器
        {
            AutomaticMigrationsEnabled = false;   //即将被微软抛弃的【自动迁移】，表现堪忧不作赘述，请老实用之前说的手动迁移
            //这里还可以执行一些其他方法，如重新设置迁移类的程序集、位置、命名空间等如：
            //this.MigrationsAssembly();
            //this.MigrationsDirectory();
            //this.MigrationsNamespace();
            //以上连最专业的EF程序员在99.99%的情况下都用不到
        }

        protected override void Seed(CodeFirstExistingDb.PlutoContext context)
        {
            //  这个方法将会在每一次update-database命令后执行
            //  其中最主要的是调用DbSet<T>.AddOrUpdate()方法对数据库经行初始化/更新，以此避免重复建立/初始化/更新原始数据
        }
    }

//Q: 为什么要用Seed方法？
//A: 一些情况下我们需要让表在每一次迁移的时候都有一些不变的原始数据，如人名、书名等等，这些数据被成为种子数据。一个让数据库拥有种子数据的原始方法
  //是在每一次迁移之前先建立一个新的空迁移文件，在该文件中使用Sql()方法和SQL语句对数据库进行插入/更改。这样做显然很麻烦，因为每一次正儿八经迁移前都要
  //建立这种带Sql()方法和语句的迁移文件。有了Seed()方法就可以一次搞定(不过用的是C#语言来初始化/更改数据库)。
  
//Q: 如何使用Seed()方法？
//A: Seed()在每一次更新数据库都会自动调用，其作用很像实例化类过程中的构造器，理论上可以往里面加各种方法满足需求，所以同构造器内容一样，Seed()方法内
  //不能储存引用型数据，如声明一个表这种是不行的。大部分时候我们只是想为数据库赋予种子数据，就需要调用AddOrUpdate()方法，这个方法在DbSet<T>下。
  //顾名思义，AddOrUpdate()方法的解释为“添加或更新”，即EF会比较数据库内容和该方法内容，如果数据库没有，则添加，如果数据库有，则更新。

//Q: 如何使用AddOrUpdate()方法？
//A: 首先来看AddOrUpdate()方法的参数：

    DbSet<T>.AddOrUpdate(Expression<Func<T, Object>> identifierExpression, params T[] entities);

  //可以看到，这个方法要求两个参数，一个是identifierExpression，Func委托返回一个Object，判定该Object是否存在，如果存在，则Add，否则Update；另一个
    //参数是entities，类型为数列，即种子数据本身。
    //这里以为Authors列表中添加作者为例：

        protected override void Seed(CodeFirstExistingDb.PlutoContext context)    //参数context为当前的PlutoContext
        {
            context.Authors.AddOrUpdate(a => a.Name, //第一个参数，检查Author表中的Name是否为存在，如果Name为不存在，则认为这是个空表，参数2将会
                                        //以添加的方式写入数据库，否则参数2以更新的方式写入数据库
                new Author  //第二个参数为一个数列，这里为Author[]数列，我们在这里写入具体的表内容，即为表Author写入种子数据
                {
                    Name = "SDW",   //Author类的两个字段，一个是Name，一个是Courses
                    Courses = new Collection<Course>()
                    {
                        new Course() { Name = "Xamarin", Description = "Nothing"}
                    }
                });
        }//这样每一次执行update-database命令后，EF会自动执行这个种子方法，赋予数据库一些种子数据，每Update一次将恢复一次种子数据

//暂时想到这么多，最后更新2118/01/03
