//Q: 什么是显示加载？
//A: 显示加载(Explicit Loading)与贪婪加载很像，指明确通知Entity Framework我们需要提前加载哪些内容。但是这两种方法的转换确是不同的。贪婪加载的原理为
  //将所有目标表使用SQL中的JOIN方法合并到一起，执行一次查询，再返回值到内存，一次往返。而显示加载的原理为不合并表，直接执行多次查询，即多次往返。

//Q: 什么情况下使用显示加载？
//A: 当我们需要提前载入多个导航数据时，如果使用贪懒加载就必须在一句中多次调用Include()方法，这将让整个查询变得臃肿。显示加载就能解决这个问题。

//Q: 如何使用显示加载？
//A: 以上例为例，如果我们要通过Authors表查询作者名下的课程，微软的官方使用指南是这么说的，代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var author = context.Authors.Single(a => a.Id == 2);    //获得Id为2的作者条目
          
            //MSDN方法
            context.Entry(author).Collection(a => a.Courses).Load();    //MSDN指示调用三个方法来实现，但这个方法仅适用于查询单个条目中的数据
        }

  //官方方法的局限很明显，仅仅支持对单个条目内容的加载，如果author为一个列表，那么MSDN所示方法则行不通。我们可以换一个角度来想这个问题，要加载指定作者
    //名下课程，也可以理解为加载所有中符合作者为指定Id的课程。取得推荐方法如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var course = context.Authors.Single(a => a.Id == 2);    //获得Id为2的作者条目
          
            //推荐方法
            context.Courses.Where(c => c.AuthorId == author.Id).Load();   //加载所有authorId为2的课程
        }

//Q: 相比贪婪加载，显示加载有哪些优势？
//A: 只要明确知道选哟提前加载哪些内容，显示加载可以通过应用过滤器方法筛选目标，以此节约大量资源。继续以上例为例，假设需要加载所有出过免费课程的作者，
  //就可以直接在查询中加入过滤器方法。这一点贪婪方法很难实现。代码如等下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var course = context.Authors.Single(a => a.Id == 2);    //获得Id为2的作者条目
          
            //MSDN方法
            context.Entry(author).Collection(a => a.Courses).Query().Where(c => c.FullPrice == 0).Load();
            //推荐方法
            context.Courses.Where(c => c.AuthorId == author.Id && c.FullPrice == 0).Load();   //加载所有authorId为2的课程
        }
