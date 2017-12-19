//Q: 如何使用代码优先工作流搭建应用程序与数据库的关系模型？
//A: 代码优先指数据库随代码变动，本身不对数据库做直接操作。实现这个机制的核心在PackageManager(包管理器)，在PM中使用命令对数据库更新。
  //对于绿地项目，即新项目，数据库同步机制如下：
  
  //前提1：安装EntityFramework。在PM中安装EntityFramework；
  //前提2：建立域。使用C#建立一个域(Domain)代表我们控制的数据库实体范围，并以集合以及类、字段的形式建立数据库中的表/列；
  //前提3：启用迁移。在PM中启用Migration(迁移)，在项目中建立Migration文件夹；
  
  //同步步骤1：建立迁移类文件。在对程序代码做一个变动之后，在PM中使用命令建立迁移类；
  //同步步骤2：调整迁移类文件。按照实际情况对迁移类的Up和Down方法，即更新和还原方法进行编辑；
  //同步步骤3：同步。确认无误后在PM中使用更新命令将数据库与程序同步，此时更新完成。

//Q: 如何用PM安装EntityFramework？
//A: 首先Tools->NuGetPackageManager->PackageManager打开PM，输入命令install-Package EntityFrameWork即可安装最新版EF，也可以在该命令后加入参数
  //命令-Version:X.X.X获取对应版本的EF。

//Q: 如何建立域和表/列？
//A: 首先以类的形式建立表中的成员，一个类即声明了一种表的格式。然后让一个集合拥有这些类，即形成一个表，EF可以识别处并自动建立表关系。
  //本篇以一个课程数据为例，数据库有三个表和一个枚举类型：
  //1.表Course，储存课程信息；
  //2.表Author，储存本课发起人信息；
  //3.表Tag储存每一种课拥有的标签；
  //4.枚举CourseLevel，以文本形式表现了课的等级，如初学者、中级、高级。
  
  //以上四项可以由类表示：

public class Course   //课程表成员的储存形式(或格式)，以类的形式表示
{
    //以下为表中的列及列的数据类型，以字段的形式储存
    public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键，也可修改，会详细讨论
    public string Title { get; set; }   //Title列，数据类型为string，这里指课程名
    public string Description { get; set; }   //Description列，数据类型为string，这里指课程的描述
    public CourseLevel Level { get; set; }    //Level列，数据类型为CourseLevel枚举类型
    public float FullPrice { get; set; }    //FullPrice列，数据类型为float
    public Author Author { get; set; }    //Author列，数据类型为Author，EF会自动将此列视为指向Author的外键(一对一或多对一)，之后会详细讨论
    public IList<Tag> Tags { get; set; }    //Tages列，数据类型为IList<Tag>，EF会自动将此列视为指向Tag的外键(多对多或一对多)，之后会详细讨论
}

public class Author   //发起者表成员的储存形式(或格式)，以类的形式表示
{
    public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键
    public string Name { get; set; }    //Name列，储存类型为string，这里指发起者的名字
    public IList<Course> Courses { get; set; }    //Courses列，数据类型为IList<Course>，EF会将其视为指向Course的外键
    //据Course类可知，Author和Course是多对一关系，即一个Course只对应一个发起者，而一个发起者可能发起多个Course
}

public class Tag    //标签表成员的储存形式(或格式)，以类的形式表示
{
    public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键
    public string Name { get; set; }    //Name列，储存类型为string，这里指标签名
    public IList<Course> Courses { get; set; }    //Course列，数据类型为IList<Course>，EF会将其视为指向Course的外键
    //据Course类可知，Tag和Course是多对多关系，即一个Course可能拥有多个Tag，而一个Tag可以往多个Course上贴
}

public enum CourseLevel   //枚举数据类型
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3
}
  
  //以上为表中的成员格式，以类的方式储存。
