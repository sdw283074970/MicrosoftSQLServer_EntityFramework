//项目需求：

//Querying Data
//Download Section 6 - Exercise Files.zip attached to this lecture and open the
//solution in Visual Studio. This is the model for Vidzy video store built using the
//code-first workflow.Whether you build a model using database-first or code-first
//workflow, querying data is exactly the same.So you can use this solution to
//exercise querying data.

//Once you open the solution, go to Tools > Package Manager > Package
//Manager Console and run Update-Database.This will generate a new database
//called Vidzy_Queries populated with some data.
//Write queries using LINQ extension methods to display:

//• Action movies sorted by name

//Output:
//Die Hard
//Terminator 2: Judgment Day
//The Dark Knight

//• Gold drama movies sorted by release date(newest first)

//Output:
//The Shawshank Redemption
//Schindlre’s List

//• All movies projected into an anonymous type with two properties:MovieName and Genre

//Output:
//The Godfather
//The Shawshank Redemption
//Schindlre’s List
//The Hangover
//Anchorman
//Die Hard
//The Dark Knight
//Terminator 2: Judgment Day

//• All movies grouped by their classification: Project the group into a new
//anonymous type with two properties: Classification(string), Movies
//(IEnumerable<Video>). For each group, display the classification and list of
//videos in that class sorted alphabetically.

//Output:
//Classification: Silver
// Anchorman
//Classification: Gold
// Die Hard
// Schindlre’s List
// The Dark Knight
// The Hangover
// The Shawshank Redemption
//Classification: Platinum
// Terminator 2: Judgment Day
// The Godfather

//• List of classifications sorted alphabetically and count of videos in them.

//Output:
//Gold(5)
//Platinum(2)
//Silver(1)

//• List of genres and number of videos they include, sorted by the number
//of videos.Genres with the highest number of videos come first.

//Output:
//Action (3)
//Drama(3)
//Comedy(2)
//Horror(0)
//Thriller(0)
//Family(0)
//Romance(0)

//代码如下：

    class Program
    {
        static void Main(string[] args)
        {
            var context = new VidzyContext();

            //Action movies sorted by name
            var moviesNameSortedQuery = context.Videos
                .Where(v => v.Genre.Name == "Action")
                .OrderBy(v => v.Name)
                .Select(v => new { MovieName = v.Name });

            foreach (var m in moviesNameSortedQuery)
                Console.WriteLine(m.MovieName);

            Console.WriteLine("=======分割线=======");

            //Gold drama movies sorted by release date (newest first)
            var goldDramaQuery = context.Videos
                .Where(v => v.Classification == Classification.Gold && v.Genre.Name == "Drama")
                .OrderByDescending(v => v.ReleaseDate)
                .Select(v => new { MovieName = v.Name });

            foreach (var g in goldDramaQuery)
                Console.WriteLine(g.MovieName);


            Console.WriteLine("=======分割线=======");

            //All movies projected into an anonymous type with two properties: MovieName and Genre
            var projectedQuery = context.Videos
                .Select(v => new { MovieName = v.Name, Genre = v.Genre.Name });

            foreach (var p in projectedQuery)
                Console.WriteLine(p.MovieName);

            Console.WriteLine("=======分割线=======");

            //All movies grouped by their classification
            var groupedClassificationQuery = context.Videos
                .GroupBy(v => v.Classification)
                .Select(g => new { Classification = g.Key.ToString(), Movies = g.OrderBy(v => v.Name) });

            foreach (var g in groupedClassificationQuery)
            {
                Console.WriteLine(g.Classification);
                foreach (var v in g.Movies)
                    Console.WriteLine("\t" + v.Name);
            }

            Console.WriteLine("=======分割线=======");

            //List of classifications sorted alphabetically and count of videos in them
            var classificationsQuery = context.Videos
                .GroupBy(v => v.Classification)
                .Select(g => new { Classification = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Classification);

            foreach (var g in classificationsQuery)
                Console.WriteLine(g.Classification + " " + g.Count);
            Console.WriteLine("=======分割线=======");

            //List of genres and number of videos they include, sorted by the numberof videos
            var genresNumberQuery = context.Genres
                .GroupJoin(context.Videos,
                    g => g.Id,
                    c => c.GenreId,
                    (Genre, Videos) => new { GenresName = Genre.Name, Count = Videos.Count() })
                .OrderByDescending(g => g.Count);

            foreach (var g in genresNumberQuery)
                Console.WriteLine("{0} ({1})", g.GenresName, g.Count);

            Console.ReadLine();

        }
    }
    
  //输出结果为：
  
    //Die Hard
    //Terminator 2: Judgment Day
    //The Dark Knight
    //=======分割线=======
    //The Shawshank Redemption
    //Schindler's List
    //=======分割线=======
    //The Godfather
    //The Shawshank Redemption
    //Schindler's List
    //The Hangover
    //Anchorman
    //Die Hard
    //The Dark Knight
    //Terminator 2: Judgment Day
    //=======分割线=======
    //Silver
    //        Anchorman
    //Gold
    //        Die Hard
    //        Schindler's List
    //        The Dark Knight
    //        The Hangover
    //        The Shawshank Redemption
    //Platinum
    //        Terminator 2: Judgment Day
    //        The Godfather
    //=======分割线=======
    //Gold 5
    //Platinum 2
    //Silver 1
    //=======分割线=======
    //Action(3)
    //Drama(3)
    //Comedy(2)
    //Horror(0)
    //Thriller(0)
    //Family(0)
    //Romance(0)
    
//最后更新01/18/2018
