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

//Q: 
