//Q: 什么是IQueryable接口？
//A: IQueryable接口继承自IEnumerable接口。IEnumerable接口让对象可以被枚举，如string、array、list、dictionary等类型，这意味着这些类型可以通过使用
  //foreach()循环来迭代。所以，IQuerable类型接口也能被枚举，这就是为什么我们能使用foreach()循环来迭代查询(query)。

//Q: IQueryable接口和IEnumerable接口有什么不同？
//A: 两者接口都可以定义查询，但IQueryable接口允许查询能被扩展，而IEnumerable接口不能让查询被扩展。允许扩展和不允许扩展区别非常大，主要是性能上的区别。
  //查询可扩展指在执行查询的时候，EF将原查询和扩展的查询合并成转换为一条SQL语句，而不可扩展的查询EF不能合并。多条语句意味着性能会被极大的浪费。
  //如以下代码为指定查询为IQueryable<Course>类型(默认也是)：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            IQueryable<Course> query = context.Courses;   //指定query的类型欸IQueryable<Course>
            var filtered = query.Where(c => c.Level == 1);    //扩展query
            var sorted = filtered.OrderBy(c => c.Name);   //再次扩展query

            foreach (var q in query)    //到这里query得以被执行，EF在后台将以上三条转换为同一条SQL执行
                Console.WriteLine(q.Name);
        }

    //以下代码为指定查询为IEnumerable<Course>类型：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            IEnumerable<Course> query = context.Courses;   //指定query的类型欸IEnumerable<Course>
            var filtered = query.Where(c => c.Level == 1);    //基于query建立新的查询
            var sorted = filtered.OrderBy(c => c.Name);   //基于filtered再次建立新的查询

            foreach (var q in query)    //到这里所有查询得以被执行，EF在将逐条执行以上三句，转换为三条SQL语句
                Console.WriteLine(q.Name);
        }

    //我们可以显而易见的看到性能的差距。第一种情况生成一条SQL语句，假设一百万条Course中仅有10条等级为1的条目，仅有这10条会被写入内存。
    //然而第二种情况生成三条SQL语句，同样假设有一百万条Course，首先这一百万条会被写入内存，然后再执行剩下的语句，最后仅剩10条。同样的结果，
      //但这就是性能上的差距。IEnumerable接口类型浪费了大量的系统资源。

//Q: IQueryable接口是如何做到允许查询扩展的？
//A: IEnumerable接口类型查询的扩展方法的参数类型直接为Func<T, T>委托，这意味着该扩展方法将在运行时立即被执行。而IQueryable接口类型查询的扩展方法
  //的参数类型为Expression<Func<T, T>>，可以看到Func<T, T>委托被一个Expression<TDelegat>对象包裹住了。Expression类对象被成为表达树，它的作用
  //是让被包裹的委托在运行时不被执行，仅仅作为一个表达保存，直到激活执行查询的时候(三种情况，上篇有细节)，Expression表达树会来一次整合，将没有被执行
  //的所有委托/匿名表达统一转换为一句SQL语句。这就是为什么IQueryable类型接口允许扩展而IEnumerable类型接口不允许的根源所在。

//暂时想到这么多，最后更新2018/01/17
