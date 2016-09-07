module Main

open Rss
open TelegramBot

open System

[<EntryPoint>]
let main argv =    
    getManyRssPosts ["https://news.google.com/?output=rss"; "http://nure.ua/category/all_news/feed/"] |> ignore   
    
    printfn "%s" test

    Console.ReadLine() |> ignore
    0