//Q: 什么是EDMX Designer？
//A: 在成功连接并导入数据库后，在解决方案面板中，会生成一个.demx文件，即DF模型对象。EDMX文件包括了所有数据库映射到应用中的信息，通过其中的.Diagram
  //可以得到一个可视化的数据库关系图。双击.edmx文件即可打开这个图，图中可以直观的看到导入的数据库中对象之间(表与表之间)的关系(如雪花关系)。这个图有点
  //类似于UML中的类图，导入的数据库对象在应用程序中也可以像源生对象一样操作、访问。
  
//Q: .edmx是什么？
//A: .edmx文件其实是一个类XML文件，分为两大块，第一块EF Runtime Content中储存了StorageModels、ConceptualModels和最重要的Mappings三个部分，
  //第二块EF Designer Content中储存了视图源码。任何在Diagram图中的变更，都会在这个文件中得到体现。用XML编辑器打开.edmx文件，我们可以看到所有的源码。
  
//Q: 什么是StorageModels、ConceptualModels和Mappings？
//A: 储存模型(StorageModels)是数据库在该应用中的具体体现，概念模型(ConceptualModels)是EF的具体体现，而映射方式(Mappings)即体现了储存模型与概念
  //模型的具体映射，也是EF的核心。

在.edmx文件中我们可以看到有两个.tt的文件，其中的.Context.tt文件为导入数据库的主对象，或者说是这个数据库本身在应用中的映射；另一个.tt文件中储存
  //了数据库中各个表的映射，在应用中以类的方式呈现。什么是.tt文件？.tt即
