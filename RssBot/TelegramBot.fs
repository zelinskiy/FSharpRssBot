module TelegramBot

open SettingsLoader
open Network
open Rss
open Database

open System
open System.Linq
open System.Threading.Tasks
open System.Text.RegularExpressions
open FSharp.Data
open FSharp.Data.JsonExtensions


//type User = { id: string; first_name: string }
//type Message = { message_id: string;  from: Option<User> ; text: string}
//type Chat = { id: string; chat_type: string }
//type Update = { update_id: string; message: Message; date: DateTime }

type Command = Subscribe | GetNews

let delay = SETTINGS.Item "Delay" |> System.Int32.Parse
let token = SETTINGS.Item "Token"
let prefix = "https://api.telegram.org/bot" + token + "/"

let cutString n (str:string) = if str.Length < n then str else str.Remove(n - 20) + "\n\n ... "
let cutMessage = cutString 4000

let makeRequest (methodName:string) (pars:list<string*string>) = 
    let request_url = prefix + methodName + makeParamsString pars
    printfn "REQ: requesting %s " request_url
    Network.getAsync request_url

let sendMessage chat_id (text:string) = makeRequest "sendMessage" [("chat_id", chat_id); ("text", cutMessage text )]

let getUpdates offset = 
    makeRequest "getUpdates" [("offset", offset.ToString()); ("timeout", "0")] 
    |> Async.RunSynchronously     
    |> JsonValue.Parse
    |> (fun x -> x?result)
    |> JsonExtensions.AsArray

let getNews n = 
    [
        "http://nure.ua/category/all_news/feed/";
        //"http://feeds.bbci.co.uk/news/rss.xml";
        "https://news.google.com/?output=rss"
    ] 
        |> Rss.getManyRssPosts 
        |> Seq.sortByDescending (fun x -> x.date)
        |> (fun seq -> if n < seq.Count() && not(Seq.isEmpty seq) then Seq.take n seq else seq)
        |> Seq.map (fun p -> "Title: " + p.title )        
        |> String.concat "\n\n"

let processCommandArguments (cmd:string) (types:array<string>) = [| "lol" |]

let processCommand command chat_id= 
    match command with
    | "/start" -> "Welcome! You are new: " + Database.addUser(chat_id).ToString()
    | "/help" -> "/help\n/subscribe\n/news"
    | "/hello" -> "Hello!"
    | cmd when cmd.StartsWith "/subscribe " -> 
        match Database.addSubscription chat_id (command.Remove(0,11)) with
        | true -> "Succesfully subscribed!"
        | false -> "Already in the Subscriptions list!"
    | "/unsubscribe" -> "Not implemented"
    | "/subscribtions" -> 
        match Database.getSubscriptions chat_id with
        | Some(res) -> String.Join("\n",res)
        | None -> "Oops!"    
    | cmd when cmd.StartsWith "/news " -> 
        try 
            let n = System.Int32.Parse(cmd.Remove(0,6))
            if n > 0 then n.ToString() + "\n" + (getNews n) else "Incorrect input: " + n.ToString()
        with
            | :? System.FormatException -> "Incorrect input!"
    | _ -> "Not a command"


let processUpdate update = 
    let chat_id = update?message?chat?id.AsString() 
    let message_text = update?message?text.AsString() 
    try
        sendMessage chat_id (processCommand message_text chat_id)
        |> Async.RunSynchronously 
        |> ignore
    with
        | :? Net.WebException -> printfn "WEBEXCEPTION"
    printfn "MSG: message from %s" chat_id
    update


let rec test (max:int) = 
    getUpdates max   
    |> Seq.map processUpdate
    |> (fun seq -> match (Seq.tryLast seq) with | None -> 0 | Some(x) -> x?update_id.AsInteger() + 1)
    |> (fun x -> (if x <> 0 then printfn "offset: %d" x else ignore x); x)
    |> (fun x -> delay |> Async.Sleep |> Async.RunSynchronously; x)
    |> test 


    
//{
//  "update_id": 996915651,
//  "message": {
//    "message_id": 2,
//    "from": {
//      "id": 274653886,
//      "first_name": "Nikita",
//      "last_name": "Yurchenko"
//    },
//    "chat": {
//      "id": 274653886,
//      "first_name": "Nikita",
//      "last_name": "Yurchenko",
//      "type": "private"
//    },
//    "date": 1473266165,
//    "text": "123123"
//  }
//}