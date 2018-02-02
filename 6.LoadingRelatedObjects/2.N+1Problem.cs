//Q: 什么是N+1问题？
//A: N+1问题是使用懒惰加载/延迟加载带来的副作用，这也是导致 Entity Framework 7 取消懒惰加载的原因之一。N+1问题指，如果在不合适的地方使用懒惰加载迭代
  //输出会造成N+1次查询，显著降低运行效率。N指迭代次数，1即迭代前的主查询。继续沿用上例举例，代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var course = context.Courses.Single(c => c.Id == 2);    //Single()方法导致此处执行一次查询，即主查询次数为1

            foreach (var t in course.Tags)    //Tags为一个集合，在course中有N个Tag，即迭代N次，每迭代一次执行一次查询
                Console.WriteLine(t.Name);
        }//于是总查询数量为N+1次

  //N+1问题会降低程序表现，在Web开发中是致命的。

//Q: 如何避免N+1问题？
//A: 使用贪婪加载或显示加载即可避免N+1问题。将在接下来的章节详述。

//暂时想到这么多，最后更新2018/02/01
