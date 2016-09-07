module RssBot

open Rss

open System.Globalization
open System.Xml
open System.Text
open System.Net
open System.IO
open System.Linq
open System.Collections.Generic
open System


let loadsettings filepath = 
    filepath
        |> File.ReadLines 
        |> Seq.map (fun s -> s.Split([|" = "|], StringSplitOptions.RemoveEmptyEntries))        
        |> ( fun x -> x.ToDictionary<string[], string, string>((fun p->p.ElementAt(0)), (fun p->p.ElementAt(1))))

let SETTINGS = loadsettings "Configuration.txt"



        


