//Q: 如何移除数据对象？
//A: 移除分两种，一种为连带删除(WithCascadeDelete)，一种为非连带删除(WithoutCascadeDelete)。连带删除指在关系型数据中，一个数据别删除后与其有关联的
  //所有数据也一并被删除。非连带删除则相反。连带删除的开关可以在覆写约定(OverrideConvince)中更改，也可以在迁移文件中设置，默认为开。删除对象同样需要将
  //被删除的对象导入内存中，然后通过调用Remove()方法删除对象，最后调用SaveChanges()保存更改。以PlutoContext为例，正常连带删除对象的代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();
            var course = context.Courses.Find(3);   //将要删除的对象实例化并导入内存，此时CT标记此对象为Unchanged
            context.Course.Remove(course);    //调用Remove()方法删除course对象，CT标记此对象为Deleted
            context.SaveChanges();    //保存更改，EF生成对应的SQL语句同步数据库
        }

  //按此方法所有依赖这个对象或这个对象依赖的对象将一起被删除。
  //如果按此方法移除禁用连带删除的对象则会报错。如，Course类中有AuthorId字段，作为外键连接的是Author的Id字段，如果没有开启连带删除，直接删除Author对象
    //的话，Course中的AuthorId就不知道指向谁了，换句话说，Author的存在依赖Course中的AuthorId字段。如果要删除一个Author对象，就必须先删除其依赖对象。

//Q: 如何移除依赖对象？
//A: 首先要贪婪加载被删除对象的依赖对象，再删除目标对象。以PlutoContext为例，要删除Id为3的Author就要先删除掉所有AuthorId为3的Course。代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();
            var author = context.Authors.Include(a => a.Courses).Single(a => a.Id == 3);    //贪婪加载Id==3的作者拥有的全部课程
            context.Courses.RemoveRange(author.Courses);    //调用RemoveRange()方法删除类型为IEnumerable<Course>的对象，即author.Courses
            context.Author.Remove(author);    //调用Remove()方法删除单一author对象
            context.SaveChanges();    //保存更改，EF生成对应的SQL语句同步数据库
        }

  //值得注意的是，必须在100%确认2次后才删除对象，推荐在逻辑上删除而不是在物理上删除对象。逻辑上删除指为某些对象声明IsDeleted字段，通过改变这个字段
    //的值来在逻辑上删除这些对象。

//暂时想到这么多，最后更新2018/02/07
