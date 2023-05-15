namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;

    public sealed class GenresController : IEntityController<Genre>, IRewritableController<Genre>
    {
        public class GenreTreeItemPresenter
        {
            private readonly Genre genre;
            private readonly int indentLevel;

            public GenreTreeItemPresenter(Genre genre, int indentLevel)
            {
                if (genre == null)
                    throw new ArgumentNullException("genre");
                if (indentLevel < 0)
                    throw new ArgumentException("indentLevel");

                this.genre = genre;
                this.indentLevel = indentLevel;
            }

            public string Description
            {
                get
                {
                    string indent = string.Empty;
                    for (int i = indentLevel; i > 0; i--)
                        indent += "-";

                    return indent + (string.IsNullOrEmpty(indent) ? string.Empty : " ") + genre.Name;
                }
            }

            public int Value
            {
                get
                {
                    return genre.Id;
                }
            }
        }

        private static class ListToTreeHelper
        {
            private class TreeItem
            {
                public Genre Genre { get; set; }

                public TreeItem Parent { get; set; }

                public List<TreeItem> Children { get; set; }

                public TreeItem() : this(null, null) { }

                public TreeItem(Genre genre) : this(genre, null) { }

                public TreeItem(Genre genre, TreeItem parent)
                {
                    Genre = genre;
                    Parent = parent;
                    Children = new List<TreeItem>();
                }
            }

            public static List<GenreTreeItemPresenter> Convert(IList<Genre> list)
            {
                TreeItem root = new TreeItem();

                int iterationCount = 0;

                while (list.Count > 0)
                {
                    // prevent hung up state
                    if (iterationCount++ == (int)short.MaxValue)
                        break;

                    List<Genre> toRemove = new List<Genre>();

                    foreach (var genre in list)
                        if (AddToTree(root, genre))
                            toRemove.Add(genre);

                    foreach (var genre in toRemove)
                        list.Remove(genre);
                }

                List<GenreTreeItemPresenter> resultList = new List<GenreTreeItemPresenter>();
                FillResultList(root, resultList, -1);
                return resultList;
            }

            private static void FillResultList(TreeItem parent, List<GenreTreeItemPresenter> resultList, int level)
            {
                if (parent == null)
                    return;

                if (parent.Genre != null)
                    resultList.Add(new GenreTreeItemPresenter(parent.Genre, level));

                parent.Children.Sort(new AlphanumericComparatorFast());

                foreach (var child in parent.Children)
                    FillResultList(child, resultList, level + 1);
            }

            private static bool AddToTree(TreeItem parent, Genre genre)
            {
                if (genre.ParentId == null || (parent.Genre != null && parent.Genre.Id == genre.ParentId))
                {
                    parent.Children.Add(new TreeItem(genre, parent));
                    return true;
                }

                foreach (var child in parent.Children)
                    if (AddToTree(child, genre))
                        return true;

                return false;
            }

            private class AlphanumericComparatorFast : IComparer<TreeItem>
            {
                public int Compare(TreeItem x, TreeItem y)
                {
                    if (x == null || x.Genre == null || y == null || y.Genre == null)
                        return 0;

                    if (x.Genre.Order != y.Genre.Order)
                        return x.Genre.Order.CompareTo(y.Genre.Order);

                    string s1 = x.Genre.Name;
                    if (string.IsNullOrEmpty(s1))
                        return 0;

                    string s2 = y.Genre.Name;
                    if (string.IsNullOrEmpty(s2))
                        return 0;

                    int len1 = s1.Length;
                    int len2 = s2.Length;
                    int marker1 = 0;
                    int marker2 = 0;

                    // Walk through two the strings with two markers.
                    while (marker1 < len1 && marker2 < len2)
                    {
                        char ch1 = s1[marker1];
                        char ch2 = s2[marker2];

                        // Some buffers we can build up characters in for each chunk.
                        char[] space1 = new char[len1];
                        int loc1 = 0;
                        char[] space2 = new char[len2];
                        int loc2 = 0;

                        // Walk through all following characters that are digits or
                        // characters in BOTH strings starting at the appropriate marker.
                        // Collect char arrays.
                        do
                        {
                            space1[loc1++] = ch1;
                            marker1++;

                            if (marker1 < len1)
                            {
                                ch1 = s1[marker1];
                            }
                            else
                            {
                                break;
                            }
                        } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                        do
                        {
                            space2[loc2++] = ch2;
                            marker2++;

                            if (marker2 < len2)
                            {
                                ch2 = s2[marker2];
                            }
                            else
                            {
                                break;
                            }
                        } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                        // If we have collected numbers, compare them numerically.
                        // Otherwise, if we have strings, compare them alphabetically.
                        string str1 = new string(space1);
                        string str2 = new string(space2);

                        int result;

                        if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                        {
                            int thisNumericChunk = int.Parse(str1);
                            int thatNumericChunk = int.Parse(str2);
                            result = thisNumericChunk.CompareTo(thatNumericChunk);
                        }
                        else
                        {
                            result = str1.CompareTo(str2);
                        }

                        if (result != 0)
                        {
                            return result;
                        }
                    }
                    return len1 - len2;
                }
            }
        }

        public IList<GenreTreeItemPresenter> GetGenresTreeViewList()
        {
            return ListToTreeHelper.Convert(Load());
        }

        public IList<Genre> GetAllGenreChildren(Genre genre)
        {
            List<Genre> genres = new List<Genre>();

            if (genre != null)
            {
                foreach (var child in genre.Children)
                {
                    genres.Add(child);
                    genres.AddRange(GetAllGenreChildren(child));
                }
            }

            return genres;
        }

        #region IEntityController<Genre> Members

        public IList<Genre> Load()
        {
            var result = from Genre genre in ContextManager.Current.Genres
                         orderby genre.Name
                         select genre;

            return result.ToList<Genre>();
        }

        public Genre Load(int id)
        {
            return ContextManager.Current.Genres.SingleOrDefault<Genre>(genre => genre.Id == id);
        }

        public void Update(Genre entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Genre entity)
        {
            throw new NotImplementedException();
        }

        #endregion IEntityController<Genre> Members

        #region IRewritableController<Genre> Members

        public Genre FindByUrlRewrite(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                return ContextManager.Current.Genres.SingleOrDefault<Genre>(genre => genre.UrlRewrite == alias);
            return null;
        }

        #endregion IRewritableController<Genre> Members
    }
}