CREATE SCHEMA butterflies2022;
USE butterflies2022;
CREATE TABLE author ( 
	authorid		INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    name			varchar(100),
	lastname		varchar(100)
);
ALTER TABLE author
CHANGE COLUMN lastname surname varchar(100) NOT NULL;

CREATE TABLE paper (
	paperID			INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    authorID		INT,
    title			varchar(225) NOT NULL,
    description		varchar(225),
    date			date,
    FOREIGN KEY (authorID)
    REFERENCES author(authorid)

);
INSERT INTO author VALUES
(1, 'Bob', 'Smith'),
(2, 'Sarah','McLaglenna');

UPDATE author
SET
name = 'Sarah',
surname = 'McLaglen'
WHERE authorid = 2;

