//Q: 什么是懒惰加载？
//A: 懒惰加载(LazyLoading)指关系型的对象(RelatedObjects)不会立即加载，只有在有需要引用的时候才会再次执行查询并加载。如以下情况：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var course = context.Courses.Single(c => c.Id == 2);    //Single()方法导致此处执行一次查询，返回IQueryable<Course>类型的对象

            //Tags为course的一个字段，当course有返回值的时候，Tags并没有，只是暂时挂起。如有需要，会重新为Tags集合专门执行查询
            foreach (var t in course.Tags)    //Tags为一个集合，每一次迭代都会重新执行一次查询并加载Tags的值到内存中
                Console.WriteLine(t.Name);
        }

//Q: 如何启用懒惰加载？
//A: 在相关字段的声明中添加virtual关键词即可。在上例中，如果我们要让Course类中的Tags集合采用懒惰加载，则只需要在Course类中的Tags字段声明中加入
  //virtual关键词。代码如下：

    public class Course
    {
        //... 该类其他部分省略
        public virtual Author Author { get; set; }    //Author启用懒惰加载
        public virtual ICollection<Tag> Tags { get; set; }    //Tags启用懒惰加载
    }

  //如此，当course被加载进内存时，Author和Tags都是没有值的，只有在需要这两个属性的时候才会再次执行查询并加载值到内存。
  
  //另外可以对是否开启懒惰加载进行全局设置。

//Q: EF是如何实现懒惰加载的？
//A: 在运行时，EF会创建一个Domain类的子类，在这里为Course的子类，叫CourseProxy。这个类会覆写所有带Virtual修饰的字段，简化代码如下：

    public override ICollection<Tag> Tags
    {
        get 
        {
            if (_tags == null)
                _tags = context.LoadTags();
                
            return _tags;
        }
    }
    
//Q: 什么时候使用懒惰加载？
//A: 主要有三种情况及其推荐使用懒惰加载：
  //1.当加载一个含复杂关系的对象非常消耗性能时。如，当一个程序启动的时候需要从数据库加载一系列数据，如果其中有个对象与其他对象的关系非常复杂，那么
    //将其一次性完全加载就非常消耗性能。这时我们可以使用懒惰加载将不用的关系型对象暂时挂起，有需要再执行查询加载；
  //2.在桌面程序中使用懒惰加载。整个本地资源都可以调用；
  //3.避免在Web程序中使用懒惰加载。这涉及到数据通信、服务器响应的问题。懒惰加载意味着每次迭代都会向服务器发送查询、 加载请求，可能会爆掉服务器。

//暂时想到这么多。最后更新2018/01/29
