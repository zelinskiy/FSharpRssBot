module TelegramBot

open SettingsLoader
open Network
open Rss

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

let makeRequest (methodName:string) (pars:list<string*string>) = 
    let request_url = prefix + methodName + makeParamsString pars
    printfn "REQ: requesting %s " request_url
    Network.getAsync request_url

let sendMessage chat_id text = makeRequest "sendMessage" [("chat_id", chat_id); ("text", text)]

let getUpdates offset = 
    makeRequest "getUpdates" [("offset", offset.ToString()); ("timeout", "0")] 
    |> Async.RunSynchronously     
    |> JsonValue.Parse
    |> (fun x -> x?result)
    |> JsonExtensions.AsArray

let getNews n = 
    ["http://nure.ua/category/all_news/feed/";
    "http://feeds.bbci.co.uk/news/rss.xml";
    "https://news.google.com/?output=rss"] 
        |> Rss.getManyRssPosts 
        |> Seq.sortByDescending (fun x -> x.date)
        |> Seq.map (fun p -> "Title: " + p.title ) 
        |> Seq.take n
        |> String.concat "\n\n"

let processCommandArguments (cmd:string) (types:array<string>) = [| "lol" |]

let processCommand command = 
    match command with
    | "/help" -> "/help\n/subscribe\n/news"
    | "/hello" -> "Hello!"
    | "/subscribe" -> "Not implemented"
    | "/unsubscribe" -> "Not implemented"
    | "/subscribtions" -> "Not implemented"
    //| cmd when Regex.Match(cmd,@"\/news( |)\d{0,2}").Success -> getNews 5
    | "/news" -> getNews 5
    | _ -> "Not a command"


let processUpdate update = 
    let chat_id = update?message?chat?id.AsString() 
    let message_text = update?message?text.AsString() 
    sendMessage chat_id (processCommand message_text)
    |> Async.RunSynchronously 
    |> ignore
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