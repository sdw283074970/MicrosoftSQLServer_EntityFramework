//Q: 什么是库模式？
//A: 库模式 RepositoryPattern 即是部署库来实现其带来的好处的具体实施。

//Q: 如何实施库模式？
//A: 首先需要库的接口IRepository和实施这个接口的库类Repository，然后再建立一个为具体实体准备的继承IRepository接口的接口(如ICourseRepository)，
  //最后建立一个继承库类Repository的具体数据库类(如CourseRepository)，并在这个类中实施刚刚建立的具体接口(如ICourseRepository)，至此库模式搭建完成。

  //以上一段话可能比较抽象，但是用图就能很好理解，如下图：
  
  //          Generic(泛型部分)                   Pluto(具体应用部分)
  //                                  继承
  //        IRepository(泛型库接口)<——————————ICourseRepository(具体实体接口)
  //                A                                A
  //                |实施接口                         |实施接口
  //                |             继承                |
  //        Repository(泛型库类)<—————————————CourseRepository(具体的实体类)

  //我们要做的就是把上图的每个部分实现。
  //如上一节所说，库只应该有Add()、Remove()等方法，不能有直接影响数据库的方法，所以在泛型库接口中只用封装这些Add()、Remove()、Get(id)、Find(predict)
    //等方法，然后在实施这个接口的泛型库中填充这些方法的具体逻辑即可完成库模式泛型部分的设计。请牢记，既然是泛型部分，那么这些与我们的应用之间没有关系，
    //我们可以将这泛型部分应用到任何其他应用中。

  //在具体应用部分，我们需要将每一个实体(Entity)都应用上库。如上图，针对Course实体，我们有ICourseRepository接口，继承自泛型库接口IRepository。
    //在这个具体实体接口中，我们拥有所有在IRepository接口中定义的方法，还可以一些针对Course实体的方法，如可以添加GetTopSellingCourses()方法、
    //GetCourseWithAuthors()方法等。另，这些接口即属于常说的业务逻辑层(BL)，它们并不关心方法的具体实施，只要结果。具体的库类就比较容易了，因为
    //其继承自泛型库类，很多代码都省略，只用填充在具体实体接口中声明的方法即可。

  //以Pluto项目为例，建立IRepository接口的代码如下：

namespace Queries.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class   //因为方法可能返回IEnumerable类型数据，所以声明泛型
    {
        TEntity Get(int id);    //声明Get(id)方法，返回TEntity类型
        IEnumerable<TEntity> GetAll();    //声明GetAll()方法，返回IEnumerable<TEntity>类型
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);   //声明Find()方法，返回IEnumerable<TEntity>类型

        void Add(TEntity entity);   //声明Add(entity)方法
        void AddRange(IEnumerable<TEntity> entities);   //声明AddRange(entities)方法

        void Remove(TEntity entity);    //声明Remove(entity)方法
        void RemoveRange(IEnumerable<TEntity> entities);    //声明RemoveRange(entities)方法
    }//此泛型接口可复用于多个应用中
}

  //然后就是建立一个执行以上接口的类Repository，代码如下：
  
namespace Queries.Core.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class   //这个方法主要是填充接口中的方法，定义其执行细节
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)    //构造器
        {
            Context = context;
        }

        public TEntity Get(int id)    //Get(id)方法执行细节
        {
            return Context.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()    //GetAll()方法执行细节
        {
            return Context.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)   //Find(predicate)方法执行细节
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public void Add(TEntity entity)   //Add(entity)方法执行细节
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)   //AddRange(entities)方法执行细节
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        public void Remove(TEntity entity)    //Remove(entity)方法执行细节
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)    //RemoveRange(entities)方法执行细节
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }
    }
}

  //接着针对具体项目的具体实体建立实体接口

//Q: 如何通过库模式写入更改数据库？
//A: 如上节所说，需要使用工作单元UnitOfWork来实现。

//Q: 如何构建工作单元？
//A: 首先需要建立工作单元的接口IUniteOfWork，这个接口应该仅针对具体应用进行设计，必须包括两个部分。一个是需要通过这个接口将需要操作的实体暴露出来，
  //二是需要至少一个与数据库沟通的方法，如Complete()。仍然以Pluto项目为例，代码如下：

namespace Queries.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository Courses { get; }    //暴露Courses实体
        IAuthorRepository Authors { get; }    //暴露Authors实体
        int Complete();   //与数据库沟通
    }
}

  //所谓暴露需要操作实体的意思为，我们可以通过实例化后的有执行IUnitOfWork的类来访问其所暴露的实体。如在Main()方法中，我们可以通过unitOfWork来访问
    //查找Id为1的课程，代码如下：
    
    static void Main(string[] args)
        {
            var unitOfWork = new UnitOfWork(new PlutoContext())
            var coursess = unitOfWork.Courses.Find(c => c.Id == 1);
        }

  //Complete()方法即与数据库沟通的方法，如可以在其中填充SaveChanges()方法等。
  //在建立完IUnitOfWork接口后，我们就需要建立一个实施这个接口的类UnitOfWork。















