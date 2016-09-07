module Main

open RssBot
open Rss
open TelegramBot

open System

[<EntryPoint>]
let main argv =     
    getManyRssPosts ["https://news.google.com/?output=rss"; "http://nure.ua/category/all_news/feed/"] 
        |> ignore   
    Console.WriteLine(SETTINGS.Item("Token"))
    Console.ReadLine() |> ignore
    0