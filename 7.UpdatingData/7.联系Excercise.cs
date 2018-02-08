//练习内容：
//Download Section 8 - Exercise Files.zip attached to this lecture and open the solution in Visual Studio. Then, go to Tools > 
//Package Manager > Package Manager Console and run Update-Database. This will generate a new database called Vidzy_UpdatingData 
//populated with some data.

  //1- Add a new video called “Terminator 1” with genre Action, release date 26 Oct, 1984, and Silver classification. Ensure the Action 
  //genre is not duplicated in the Genres table.

  //2- Add two tags “classics” and “drama” to the database. Ensure if your method is called twice, these tags are not duplicated.

  //3- Add three tags “classics”, “drama” and “comedy” to the video with Id 1 (The Godfather). Ensure the “classics” and “drama” tags are
  //not duplicated in the Tags table. Also, ensure that if your method is called twice, these tags are not duplicated in VideoTags table.

  //4- Remove the “comedy” tag from the the video with Id 1 (The Godfather).

  //5- Remove the video with Id 1 (The Godfather).

  //6- Remove the genre with Id 2 (Action). Ensure all courses with this genre are deleted from the database. 

//自写代码如下：

    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new VidzyContext())
            {
                //Add a new video
                var genre = context.Genres.Single(g => g.Name == "Action");
                var newVideo = new Video()
                {
                    Name = "Terminator 1",
                    ReleaseDate = DateTime.Today,
                    Classification = Classification.Silver,
                    Genre = genre
                };
                context.Videos.Add(newVideo);
                context.SaveChanges();

                //Add two tags
                var tag_1 = new Tag()
                {
                    Name = "Classics"
                };
                var tag_2 = new Tag()
                {
                    Name = "Drama"
                };
                context.Tags.Add(tag_1);
                context.Tags.Add(tag_2);
                context.SaveChanges();

                //Add three tags to the video with Id 1
                var tag_3 = new Tag()
                {
                    Name = "Comedy"
                };
                context.Tags.Add(tag_3);
                var tags = context.Tags.ToList();
                var video_1 = context.Videos.Single(v => v.Id == 1);
                video_1.AddTag(tag_1);
                video_1.AddTag(tag_2);
                video_1.AddTag(tag_3);
                context.SaveChanges();

                //Remove the “comedy” tag from the the video with Id 1
                context.Tags.Where(t => t.Name == "Comedy").Load();
                var targetVideo = context.Videos.Single(v => v.Id == 1);
                targetVideo.RemoveTag("Comedy");
                context.SaveChanges();

                //Remove the video with Id 1
                var targetVideo_1 = context.Videos.Single(v => v.Id == 1);
                context.Videos.Remove(targetVideo_1);
                context.SaveChanges();

                //Remove the genre with Id 2 (Action)
                var targetGenre = context.Genres.Include(g => g.Videos).Single(g => g.Id == 2);
                context.Videos.RemoveRange(targetGenre.Videos);
                context.Genres.Remove(targetGenre);
                context.SaveChanges();
            }
        }
    }
    
//以上为一次性代码。也可以将这些执行过程封装到方法中方便下次使用。封装成方法后的代码如下：

class Program
    {
        static void Main(string[] args)
        {
            // 使用Using代码快保证在结束后释放连接资源
            // 在每一个方法中都重新声明context，这并不影响功能使用

            // Exercise 1: Add a new video (Terminator 1)
            
            // Here I have hardcoded the GenreId (2). In a real-world application,
            // the user often selects the genre from a drop-down list. There, you should
            // have the Id for each genre. If you're building a WPF application, this 
            // GenreId is already in the memory. If you're building an ASP.NET MVC application,
            // the GenreId is posted with the request and you can set it here. 
            AddVideo(new Video
            {
                Name = "Terminator 1",
                GenreId = 2,
                Classification = Classification.Silver,
                ReleaseDate = new DateTime(1984, 10, 26)
            });


            // Exercise 2: Add two tags "classics" and "drama" to the database.
            AddTags("classics", "drama");


            // Exercise 3: Add three tags "classics", "drama" and "comedy" to video with Id 1.
            AddTagsToVideo(1, "classics", "drama", "comedy");


            // Exercise 4: Remove the "comedy" tag from Video with Id 1. 
            RemoveTagsFromVideo(1, "comedy");


            // Exercise 5: Remove the video with Id 1.
            RemoveVideo(1);


            // Exercise 6: Remove the genre with Id 2.
            // .
            // Note use of named parameter here to improve the readability of the code. 
            // Without this, my code would look like: 
            //
            // RemoveGenre(2, true); // What does true mean here? 
            RemoveGenre(2, enforceDeletingVideos: true);
        }

        public static void AddVideo(Video video)
        {
            using (var context = new VidzyContext())
            {
                context.Videos.Add(video);
                context.SaveChanges();
            }
        }

        public static void AddTags(params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                // We load the tags with the given names first, to prevent adding duplicates.
                var tags = context.Tags.Where(t => tagNames.Contains(t.Name)).ToList();

                foreach (var name in tagNames)
                {
                    if (!tags.Any(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
                        context.Tags.Add(new Tag { Name = name });
                }

                context.SaveChanges();
            }
        }

        // 从API设计的角度, 将目标tag的name以string类型传入要好很多。
        // 我们不应该使用TagId因为这样的话方法仅会在tag存在数据库的情况下才能工作。
        // 为了用户体验，用户应该能传入他们想要的任何tag名称，或者同时添加或对一个已存在的video经行编辑。
        // 所以我们应该使用string作为参数传入到方法中。 另外，tag的name应该是独一无二的。
        public static void AddTagsToVideo(int videoId, params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                // This technique with LINQ leads to 
                // 
                // SELECT FROM Tags WHERE Name IN ('classics', 'drama')
                var tags = context.Tags.Where(t => tagNames.Contains(t.Name)).ToList();

                // So, first we load tags with the given names from the database 
                // to ensure we won't duplicate them. Now, we loop through the list of
                // tag names, and if we don't have such a tag in the database, we add
                // them to the list.
                foreach (var tagName in tagNames)
                {
                    if (!tags.Any(t => t.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase)))
                        tags.Add(new Tag { Name = tagName });
                }

                var video = context.Videos.Single(v => v.Id == videoId);

                tags.ForEach(t => video.AddTag(t));

                context.SaveChanges();
            }
        }

        public static void RemoveTagsFromVideo(int videoId, params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                // We can use explicit loading to only load tags that we're going to delete.
                context.Tags.Where(t => tagNames.Contains(t.Name)).Load();

                var video = context.Videos.Single(v => v.Id == videoId);

                foreach (var tagName in tagNames)
                {
                    // I've encapsulated the concept of removing a tag inside the Video class. 
                    // This is the object-oriented way to implement this. The Video class
                    // should be responsible for adding/removing objects to its Tags collection. 
                    video.RemoveTag(tagName);
                }

                context.SaveChanges();
            }
        }

        public static void RemoveVideo(int videoId)
        {
            using (var context = new VidzyContext())
            {
                var video = context.Videos.SingleOrDefault(v => v.Id == videoId);
                if (video == null) return;

                context.Videos.Remove(video);
                context.SaveChanges();
            }
        }

        public static void RemoveGenre(int genreId, bool enforceDeletingVideos)
        {
            using (var context = new VidzyContext())
            {
                var genre = context.Genres.Include(g => g.Videos).SingleOrDefault(g => g.Id == genreId);
                if (genre == null) return;

                if (enforceDeletingVideos)
                    context.Videos.RemoveRange(genre.Videos);

                context.Genres.Remove(genre);
                context.SaveChanges();
            }
        }
    }

//暂时想到这么多，最后更新2018/02/07
