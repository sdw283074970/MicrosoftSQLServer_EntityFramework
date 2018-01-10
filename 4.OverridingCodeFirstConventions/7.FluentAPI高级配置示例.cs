//Q: 什么是高级配置？
//A: 当出现多对多关系时，会生成一个中间表，覆写这个中间表用传统的方法行不通，因为这个中间表在Model中没有具体的对象，也就没办法覆写。唯一的解决方案
  //是选择多对多关系(这时等于获得了中间表的引用)，再调用高级方法覆写。在之前FluentAPI示例中就有表现，这里详细说明一下。

  //在项目需求“4.将Courses与Tags的中间表命名为“CourseTags”而不是默认的“TagCourses”，并拿掉这个中间表外键中的下划线”就是通过调用Map()方法完成覆写。
    //解决代码如下：

            modelBuilder.Entity<Course>()       //选择Course为起始类
                .HasMany(c => c.Tags)       //一个Course有很多Tag
                .WithMany(t => t.Courses)       //一个Tage有很多Course
                .Map(m =>       //调用Map方法，其参数为一个Action<ManyToManyAssociationMappingConfiguration>委托
                    {       //ManyToManyAssociationMappingConfiguration类中有4个方法，这里我们用到其中三个
                        m.ToTable("CourseTags");   //有ToTable(string tableName)和ToTable(string tableName string schemeName)两个重载
                        m.MapLeftKey("CourseId");   //MapLeftKey(string[] keyColumnNames)支持为多个外键命名，LeftKey即起始类(Course)的外键
                        m.MapRightKey("TagId");     //MapRightKey(string[] keyColumnNames)支持为多个外键命名，RightKey即终点类(Tag)的外键
                    });

  //这里近距离观察一下ManyToManyNavigationPropertyConfiguration类，这个类的源代码微软写的，不开源，内容我们看不到，但是能看到签名，伪源代码如下：

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
    public class ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType>
        where TEntityType : class
        where TTargetEntityType : class
    {
        public override bool Equals(object obj);
          
        public override int GetHashCode();
          
        public Type GetType();
          
        //我们用的Map方法，返回的是ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType>配置类对象
        public ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> Map(Action<ManyToManyAssociationMappingConfiguration> configurationAction);
          
        public ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> MapToStoredProcedures();
          
        public ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> MapToStoredProcedures(Action<ManyToManyModificationStoredProceduresConfiguration<TEntityType, TTargetEntityType>> modificationStoredProcedureMappingConfigurationAction);
          
        public override string ToString();
    }
}

  //但是Map方法中Action<T>委托传入的ManyToManyAssociationMappingConfiguration类我们通过Object浏览器至少直到其中有4个方法，其中ToTable()方法有
    //两个重载，它们分别是：

    ManyToManyAssociationMappingConfiguration.HasTableAnnotation(string annotationName, object annotationValue);
    ManyToManyAssociationMappingConfiguration.MapLeftKey(string[] keyColumnNames);
    ManyToManyAssociationMappingConfiguration.MapRightKey(string[] keyColumnNames);
    ManyToManyAssociationMappingConfiguration.ToTable(string tableName);
    ManyToManyAssociationMappingConfiguration.ToTable(string tableName, string schemeName);

  //在上例中，为了更改中间表的表名我们调用了ToTable()方法；更改表中的联合主键名我们接着链式调用了MapLeftKey()和MapRightKey()方法。
    //由于Action<T>委托是无返回值委托，所以我们可以按需求一直链式调用下去。

//暂时想到这么多，最后更新2018/01/09
