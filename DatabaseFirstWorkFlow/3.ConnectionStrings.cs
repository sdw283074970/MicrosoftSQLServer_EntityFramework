//Q: 应用程序项目是如何知道连接那个服务器的哪一个数据库的？
//A: 所有连接设置都在应用程序解决方案中的App.config中。App.config是一个XML文件，储存的是带标签的文本数据，通过识别标签，应用程序就能找到对应的文本，
  //并将其转换为计算机语言。其中，<connectionstrings></connectionstrings>就是这样一个标签，其中储存的文本数据则不仅指向了连接目标，还将附带的必要
  //属性如访问权限也一并包括了。

//Q: 这个connectionStrings标签内容是自动生成的么？
//A: 在DatabaseFirst Workflow中为自动生成，而CodeFirst Workflow中得手写，并且两者的标签内容有些许不同，主要集中在connectionString属性上。
  DFW中自动生成的connectionStrings文本数据如下：

<connectionStrings>
  <add name="DatabaseEntity" connectionString="metadata=res://*/xxx.csdl|res://*/xxx.ssdl|res://*/xxx.msl;provider=System.Data.SqlClient;
    provider connection string=&quot;data source=[SQLSERVER NAME];initial catalog=[SQLSERVER DATABASE];integrated security=True;
    multipleactiveresultsets=True;application name=EntityFramework&quot;" providerName="System.Data.EntityClient"/>
</connectionStrings>

  //我们可以看到，自动生成的文本数据看起来非常复杂，尤其是元数据(metadata)部分。整个connectionStrings的add属性其实可以简单理解为三部分（其中add
    //本身的意思就是添加连接）：
    //第一部分，name属性。这里的名字为服务器数据库映射到解决方案中的实体名，在CodeFirst Workflow中可以随便写，只用在数据库主类中构造器声明一下就能
      //正常连接，而DFW中的这个name属性为自动生成，与自动生成的数据库映射到应用中的主类名称一样；
    //第二部分，connectionString属性。其中的第一项元数据指向的就是edmx文件中的三个部分，即StorageModels(xxx.ssdl)、ConceptualModels(xxx.csdl)以及
      //Mappings(xxx.msl)三个部分。往后的provider其实跟最后一个属性providerName重复，CFW中可以省略，provider connection string为&quot;到下一个
      //&quot;之间的文本。即data source指明数据源，指服务器名。initial catalog指初始化的目录，即服务器中的数据库名。integrated security指访问权限，
      //True指开放权限，也可以设置为SSIP即Window本地权限。multipleactiveresultsets之后到&quot部分的属性都可以省略；
    //第三部分，providerName。这一句声明将要连接的数据库类型是SQLSERVER数据库。
  //在下一部分的CodeFirst Workflow中可以将ConnectionStrings简化书写。我们这里只要了解这些元数据指向的是什么东西就可以了。
  
//暂时想到这么多，最后更新2017/12/15
