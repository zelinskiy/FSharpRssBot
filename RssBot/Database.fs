module Database


open FSharp.Data

open System.Data.SqlClient
open System.Linq

type User = { Name:string; Subscriptions:List<string>}

let private parseSubscriptionsString (str:string) = 
    str.Split([|','|]) 
    |> Seq.tail 
    |> Seq.toList

[<Literal>]
let connectionString = 
    @"Data Source=localhost\SQLEXPRESS;" + 
    "Initial Catalog=RssBotDb;" + 
    "Integrated Security=True;" +
    "MultipleActiveResultSets=True;"

let addUser (username:string) = 
    use cmd = new SqlCommandProvider<"
        INSERT dbo.Persons(Name, Subscriptions)
        VALUES(@_username, '')
        " , connectionString>(connectionString)
    try
        cmd.Execute(username) |> ignore
        true
    with
        | :? SqlException as ex -> false

let addSubscription username subname = 
    use cmd = new SqlCommandProvider<"
        DECLARE @subname varchar(255);
        SET @subname = @_subname;

        DECLARE @username varchar(255);
        SET @username = @_username;

        IF EXISTS
        (
	        SELECT * from dbo.Persons
	        WHERE Name = @username
	        AND NOT Subscriptions LIKE CONCAT('%', @subname, '%')
        )
        BEGIN
            UPDATE dbo.Persons
	        SET Subscriptions = CONCAT(Subscriptions, CONCAT(',', @subname))
	        WHERE Name = @username
        END
        " , connectionString>(connectionString)
    try
        cmd.Execute(_username = username, _subname = subname) |> (=) 1
    with
        | :? SqlException as ex -> false

let removeSubscription username subname = 
    use cmd = new SqlCommandProvider<"
        DECLARE @subname varchar(255);
        SET @subname = @_subname;

        DECLARE @username varchar(255);
        SET @username = @_username;

        IF EXISTS
        (
	        SELECT * from dbo.Persons
	        WHERE Name = @username
	        AND Subscriptions LIKE CONCAT('%', @subname, '%')
        )
        BEGIN
            UPDATE dbo.Persons
	        SET Subscriptions = REPLACE(Subscriptions, CONCAT(',', @subname), '')
	        WHERE Name = @username
        END
        " , connectionString>(connectionString)
    try
        cmd.Execute(_username = username, _subname = subname) |> (=) 1
    with
        | :? SqlException as ex -> false

let getSubscriptions username = 
    use cmd = new SqlCommandProvider<"
        SELECT Subscriptions from dbo.Persons
        WHERE Name = @_username
        ", connectionString>(connectionString)
    match cmd.Execute(_username = username) |> Seq.toList  with
    | [subs]-> subs |> parseSubscriptionsString |> Some
    | _ -> None
    
    
        

let allUsers = 
    use cmd = new SqlCommandProvider<"
        SELECT * FROM Persons
        " , connectionString>(connectionString)
    cmd.Execute()
        |> Seq.map (fun x -> 
            { 
                Name = x.Name; 
                Subscriptions = parseSubscriptionsString x.Subscriptions
            })