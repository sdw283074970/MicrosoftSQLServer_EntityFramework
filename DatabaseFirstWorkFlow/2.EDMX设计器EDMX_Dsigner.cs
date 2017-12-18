//Q: 什么是EDMX Designer？
//A: 在成功连接并导入数据库后，在解决方案面板中，会生成一个.demx文件，即DF模型对象。EDMX文件包括了所有数据库映射到应用中的信息，通过其中的.Diagram
  //可以得到一个可视化的数据库关系图。双击.edmx文件即可打开这个图，图中可以直观的看到导入的数据库中对象之间(表与表之间)的关系(如雪花关系)。这个图有点
  //类似于UML中的类图，导入的数据库对象在应用程序中也可以像源生对象一样操作、访问。
  
//Q: .edmx是什么？
//A: .edmx文件其实是一个自动生成的XML文件，分为两大块，第一块EF Runtime Content中储存了StorageModels、ConceptualModels和最重要的Mappings三个部分，
  //第二块EF Designer Content中储存了视图源码。任何在Diagram图中的变更，都会在这个文件中得到体现。用XML编辑器打开.edmx文件，我们可以看到所有的源码。
  //除此之外，在解决方案面板中也可以之间展开.edmx文件，展开后我们可以看到有两个.tt的文件、一个.Designer.cs文件和一个.edmx.diagram文件。

//Q: 什么是.tt文件？
//A: .tt代表T4 Template，即这是一个模板文件。EF读取数据库数据，通过这个模板文件，自动将数据库翻译生成C#类文件。这里有两个tt文件，
  //第一个tt文件即XXX.Context.tt文件展开后的C#类文件为导入数据库的主对象，或者说是这个数据库本身在应用中的映射，即调用这个生成的类即导入的数据库本身，
    //实例化这个类即实例化了一个数据库对象，而后即可用C#的方法对这个对象经行操作；
  //第二个tt文件包含了数据库中表，EF通过T4模板也将其翻译成了类，类中的属性也就是表中列的的属性。
  
//Q: 什么是StorageModels、ConceptualModels和Mappings？
//A: 储存模型(StorageModels)是数据库在该应用中的具体体现，概念模型(ConceptualModels)是EF的具体体现，而映射方式(Mappings)即体现了储存模型与概念
  //模型的具体映射，也是EF这种ORM(Object-Relational Mapper)的核心，通过Mappings，ORM就能知道如何将一个面向对象的模型映射向关系型模型。
  //我们分开研究这三个东西到底储存了什么：
  //1.在StorageModels中，储存了所有有关于数据库信息的数据，包括表、键、表中的列、列的属性等，是数据库以XML代码形式的具体体现；
  //2.在ConceptualModels中，储存了数据库的映射信息，我们主要在ConceptualModels中经行编辑、修改等操作；
  //3.在Mappings中， 储存在ConceptualModels中属性与StorageModels中属性的映射方式，即确保我们在ConceptualModels中做的改变会正确的改变StorageModels
    //中相应的地方。

//Q: 既然ConceptualModels有可视化的图，那Mappings有可视化的操作地方吗？
//A: 是可以的。回到EDMX Designer，图中展示的都是ConceptualModels的对象/实体，即对这个图的操作将通过StorageModels映射到数据库中。在图中右键一个对象
  //选择表映射(Table Mapping)即可调出映射属性框，在属性框中，我们可以修改对象，即修改表映射和表中的列映射。什么时候需要这种操作？如，因为表太复杂了，
  //我们想让EF忽略一些列映射，就可以在表中将ConceptualModels中的列与StorageModels中的列的映射删除掉。这样在ConceptualModels对该列做出的任何改变，
  //将无法影响到数据库中的表。

//Q: 那么StorageModels是否也有可视化图呢？
//A: 也是有的。右键设计图空旷处，点击MOdel View，我们就能看到Model.Storage的目录图。

//暂时想到这么多，最后更新2017/12/14
