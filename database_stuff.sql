CREATE DATABASE LibraryProjectDatabase;
GO

USE LibraryProjectDatabase;
GO

CREATE TABLE Users (
	UserId INT IDENTITY(1, 1) PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100)
);

CREATE TABLE UsersAuth (
    UserId INT,
    PasswordHash VARBINARY(MAX),
    PasswordSalt VARBINARY(MAX),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE Books (
    BookId INT IDENTITY(1, 1) PRIMARY KEY,
    Title NVARCHAR(100),
    Author VARCHAR(100),
    ISBN VARCHAR(20),
    CopiesAvailable INT
);

CREATE TABLE Loan (
    LoanId INT IDENTITY(1, 1) PRIMARY KEY,
    UserId INT,
    BookId INT,
    LoanDate DATETIME,
    ReturnDate DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);
GO

INSERT INTO Users (FirstName, LastName, Email)
VALUES
('John', 'Doe', 'john.doe@example.com'),
('Jane', 'Smith', 'jane.smith@example.com'),
('Michael', 'Johnson', 'michael.johnson@example.com'),
('Emily', 'Williams', 'emily.williams@example.com'),
('William', 'Brown', 'william.brown@example.com'),
('Emma', 'Jones', 'emma.jones@example.com'),
('James', 'Garcia', 'james.garcia@example.com'),
('Olivia', 'Miller', 'olivia.miller@example.com'),
('Benjamin', 'Davis', 'benjamin.davis@example.com'),
('Evelyn', 'Martinez', 'evelyn.martinez@example.com'),
('Elijah', 'Hernandez', 'elijah.hernandez@example.com'),
('Charlotte', 'Lopez', 'charlotte.lopez@example.com'),
('Lucas', 'Gonzalez', 'lucas.gonzalez@example.com'),
('Amelia', 'Wilson', 'amelia.wilson@example.com'),
('Mason', 'Anderson', 'mason.anderson@example.com'),
('Harper', 'Thomas', 'harper.thomas@example.com'),
('Ethan', 'Taylor', 'ethan.taylor@example.com'),
('Ava', 'Moore', 'ava.moore@example.com'),
('Logan', 'Jackson', 'logan.jackson@example.com'),
('Sophia', 'White', 'sophia.white@example.com'),
('Liam', 'Harris', 'liam.harris@example.com'),
('Isabella', 'Martin', 'isabella.martin@example.com'),
('Noah', 'Thompson', 'noah.thompson@example.com'),
('Avery', 'Garcia', 'avery.garcia@example.com'),
('Grace', 'Rodriguez', 'grace.rodriguez@example.com');

INSERT INTO Books (Title, Author, ISBN, CopiesAvailable)
VALUES
('To Kill a Mockingbird', 'Harper Lee', '9780061120084', 5),
('1984', 'George Orwell', '9780451524935', 8),
('The Great Gatsby', 'F. Scott Fitzgerald', '9780743273565', 3),
('Pride and Prejudice', 'Jane Austen', '9780141439518', 6),
('The Catcher in the Rye', 'J.D. Salinger', '9780316769488', 4),
('Animal Farm', 'George Orwell', '9780451526342', 7),
('Brave New World', 'Aldous Huxley', '9780060850524', 2),
('To the Lighthouse', 'Virginia Woolf', '9780156907392', 5),
('Lord of the Flies', 'William Golding', '9780399501487', 3),
('Catch-22', 'Joseph Heller', '9781451626650', 6),
('The Hobbit', 'J.R.R. Tolkien', '9780547928227', 4),
('The Grapes of Wrath', 'John Steinbeck', '9780143039433', 8),
('Moby-Dick', 'Herman Melville', '9780142437247', 5),
('One Hundred Years of Solitude', 'Gabriel Garcia Marquez', '9780060883287', 7),
('Beloved', 'Toni Morrison', '9781400033416', 3),
('The Picture of Dorian Gray', 'Oscar Wilde', '9780486278070', 4),
('Slaughterhouse-Five', 'Kurt Vonnegut', '9780385333849', 6),
('Frankenstein', 'Mary Shelley', '9780141439471', 5),
('Wuthering Heights', 'Emily Bronte', '9780141439556', 2),
('Jane Eyre', 'Charlotte Bronte', '9780141441146', 4),
('The Odyssey', 'Homer', '9780140268867', 6),
('Crime and Punishment', 'Fyodor Dostoevsky', '9780140449136', 3),
('Anna Karenina', 'Leo Tolstoy', '9780143035008', 7),
('The Road', 'Cormac McCarthy', '9780307387899', 5),
('Invisible Man', 'Ralph Ellison', '9780679732761', 4),
('The Brothers Karamazov', 'Fyodor Dostoevsky', '9780374528379', 6),
('A Tale of Two Cities', 'Charles Dickens', '9780141439600', 3),
('Gulliver''s Travels', 'Jonathan Swift', '9780141439495', 5),
('The Scarlet Letter', 'Nathaniel Hawthorne', '9780142437261', 2),
('The Count of Monte Cristo', 'Alexandre Dumas', '9780140449266', 4);

INSERT INTO Loan (UserId, BookId, LoanDate, ReturnDate)
VALUES
(1, 1, '2024-06-01', '2024-06-15'),
(2, 3, '2024-06-02', '2024-06-16'),
(3, 5, '2024-06-03', '2024-06-17'),
(4, 7, '2024-06-04', '2024-06-18'),
(5, 9, '2024-06-05', '2024-06-19'),
(6, 11, '2024-06-06', '2024-06-20'),
(7, 13, '2024-06-07', '2024-06-21');


GO


CREATE OR ALTER PROCEDURE spUsers_Get
    @UserId INT = NULL,
    @Email  VARCHAR(100) = NULL
AS
BEGIN
    SELECT [UserId], [FirstName], [LastName], [Email]
    FROM Users
    WHERE UserId = ISNULL(@UserId, UserId)
        AND Email = ISNULL(@Email, Email)
END;
GO

CREATE OR ALTER PROCEDURE spUsers_Upsert
    @FirstName VARCHAR(50),
    @LastName  VARCHAR(50),
    @Email     VARCHAR(100),
    @UserId    INT = 0
AS
BEGIN
    IF EXISTS (SELECT * FROM Users WHERE UserId = @UserId)
        BEGIN
            UPDATE Users
            SET FirstName = @FirstName,
                LastName = @LastName,
                Email = @Email
            WHERE UserId = @UserId
        END
    ELSE
        BEGIN
            INSERT INTO Users(FirstName, LastName, Email)
            VALUES(@FirstName, @LastName, @Email)
        END
END;
GO

CREATE OR ALTER PROCEDURE spUsers_Delete
    @UserId INT = NULL
AS
BEGIN
    DELETE FROM UsersAuth WHERE UserId = @UserId

    DELETE FROM Users WHERE UserId = @UserId
END;
GO

CREATE OR ALTER PROCEDURE spAuth_Upsert
    @UserId INT,
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
BEGIN
    IF EXISTS (SELECT * FROM UsersAuth WHERE UserId = @UserId)
        BEGIN
            UPDATE UsersAuth
            SET PasswordHash = @PasswordHash,
                PasswordSalt = @PasswordSalt
            WHERE UserId = @UserId
        END
    ELSE
        BEGIN
            INSERT INTO UsersAuth(UserId, PasswordHash, PasswordSalt)
            VALUES(@UserId, @PasswordHash, @PasswordSalt)
        END
END;
GO

CREATE OR ALTER PROCEDURE spAuth_Get
    @Email VARCHAR(100)
AS
BEGIN
    DECLARE @UserId INT
    SET @UserId = (SELECT UserId FROM Users WHERE Email = @Email)

    SELECT [PasswordHash], [PasswordSalt] FROM UsersAuth
    WHERE UserId = @UserId
END;
GO

CREATE OR ALTER PROCEDURE spUserRegister
    @FirstName VARCHAR(50),
    @LastName  VARCHAR(50),
    @Email     VARCHAR(100),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
BEGIN
    EXEC spUsers_Upsert @FirstName, @LastName, @Email;

    DECLARE @OutputUserId INT
    SET @OutputUserId = @@IDENTITY

    EXEC spAuth_Upsert @OutputUserId, @PasswordHash, @PasswordSalt
END;
GO

CREATE OR ALTER PROCEDURE spBooks_Get
    @Title VARCHAR(100) = NULL,
    @Author VARCHAR(100) = NULL,
    @ISBN VARCHAR(20) = NULL,
    @BookId INT = NULL
AS
BEGIN
    SELECT [BookId], [Title], [Author], [ISBN], [CopiesAvailable]
    FROM Books
    WHERE (@Title IS NULL OR Title LIKE '%' + @Title + '%')
        AND (@Author IS NULL OR Author LIKE '%' + @Author + '%')
        AND (@ISBN IS NULL OR ISBN LIKE '%' + @ISBN + '%')
        AND BookId = ISNULL(@BookId, BookId)
END;
GO

CREATE OR ALTER PROCEDURE spBooks_Upsert
    @Title VARCHAR(100),
    @Author VARCHAR(100),
    @ISBN VARCHAR(20),
    @Copies INT,
    @BookId INT = NULL
AS
BEGIN
    IF EXISTS (SELECT * FROM Books WHERE BookId = @BookId)
    BEGIN
        UPDATE Books
        SET Title = @Title,
            Author = @Author,
            ISBN = @ISBN,
            CopiesAvailable = @Copies
        WHERE BookId = @BookId
    END
    ELSE
    BEGIN
        INSERT INTO Books ([Title], [Author], [ISBN], [CopiesAvailable])
        VALUES (@Title, @Author, @ISBN, @Copies)
    END
END;
GO

CREATE OR ALTER PROCEDURE spBooks_Delete
    @BookId INT
AS
BEGIN
    DELETE FROM Books WHERE BookId = @BookId
END;
GO


CREATE OR ALTER PROCEDURE spUsersLoans_Get
    @UserId INT = NULL,
    @BookId INT = NULL,
    @LoanId INT = NULL,
    @LoanStartDate DATE = NULL,
    @LoanEndDate DATE = NULL,
    @HasReturned BIT = 0
AS
BEGIN
    DECLARE @ReturnDate DATETIME
    IF (@HasReturned = 1)
    BEGIN
        SET @ReturnDate = GETDATE()
    END
    ELSE
    BEGIN
        SET @ReturnDate = (SELECT CAST(-53690 as datetime))
    END

    IF (@LoanEndDate IS NULL)
    BEGIN
        SET @LoanEndDate = DATEADD(day, 1, GETDATE())
    END

    IF (@LoanStartDate IS NULL)
    BEGIN
        SET @LoanStartDate = (SELECT CAST(-53690 as datetime))
    END

    SELECT [Users].[UserId], [Users].[FirstName], [Users].[LastName], [Users].[Email],
        [Books].[BookId], [Books].[Title], [Books].[Author],
        [Loan].[LoanDate], [Loan].[ReturnDate]
    FROM Users
    JOIN Loan
        ON Loan.UserId = Users.UserId
    JOIN Books
        ON Loan.BookId = Books.BookId
    WHERE Users.UserId = ISNULL(@UserId, Users.UserId)
        AND Loan.LoanId = ISNULL(@LoanId, Loan.LoanId)
        AND Books.BookId = ISNULL(@BookId, Books.BookId)
        AND LoanDate BETWEEN @LoanStartDate AND @LoanEndDate
        AND (@HasReturned = 0 OR ReturnDate <= GETDATE())
END;
GO

CREATE OR ALTER PROCEDURE spLoan_Upsert
    @UserId INT,
    @BookId INT,
    @LoanId INT = NULL,
    @IsReturning BIT = 0
AS
BEGIN
    IF (@IsReturning = 1) AND EXISTS (SELECT * FROM Loan WHERE LoanId = @LoanId)
    BEGIN
        UPDATE Loan
        SET ReturnDate = GETDATE()
        WHERE LoanId = @LoanId

        UPDATE Books
        SET CopiesAvailable = CopiesAvailable + 1
        WHERE BookId = @BookId
    END
    ELSE
    BEGIN
        IF (@IsReturning = 0) AND ((SELECT CopiesAvailable FROM Books WHERE BookId = @BookId) > 0)
        BEGIN
            INSERT INTO Loan ([UserId], [BookId], [LoanDate])
            VALUES (@UserId, @BookId, GETDATE())

            UPDATE Books
            SET CopiesAvailable = CopiesAvailable - 1
            WHERE BookId = @BookId
        END
    END
END;
GO

CREATE OR ALTER PROCEDURE spLoan_Delete
    @LoanId INT
AS
BEGIN
    DELETE FROM Loan WHERE LoanId = @LoanId
END;
GO