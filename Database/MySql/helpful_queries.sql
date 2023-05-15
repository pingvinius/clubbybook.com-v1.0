-- select last N confirmed books
SELECT concat('- ', b.BookName, ' (', b.AuthorName, ');')
FROM
  (
  SELECT book.Title AS BookName
       , author.FullName AS AuthorName
  FROM
    book, author, bookauthor
  WHERE
    book.Confirmed = 1
    AND bookauthor.AuthorId = author.Id
    AND bookauthor.BookId = book.Id
  ORDER BY
    book.CreatedDate DESC
  LIMIT
    100) AS b
ORDER BY
  b.BookName;

-- select last N authors
SELECT a.Name
FROM
  (
  SELECT author.FullName AS Name
  FROM
    author
  ORDER BY
    author.CreatedDate DESC
  LIMIT
    100) AS a
ORDER BY
  a.Name;


-- Genres
SELECT g.Name
     , count(b.Id)
     , concat(g.Name, ' - ', count(b.Id), ' шт.')
FROM
  BookGenre bg
INNER JOIN Genre g
ON g.Id = bg.GenreId
INNER JOIN Book b
ON b.Id = bg.BookId
GROUP BY
  g.Id
ORDER BY
  count(b.Id)
  DESC
, g.Name; 