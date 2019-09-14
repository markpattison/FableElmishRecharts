module App.Markdown

open System.Text.RegularExpressions
open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props



module private Util =
    let isAbsoluteUrl (url: string) =
            Regex.IsMatch(url, @"^(?:[a-z]+:)?//", RegexOptions.IgnoreCase)
    
    let marked : obj = importDefault "marked"
    
    let renderer = createNew marked?Renderer ()

    renderer?heading <- fun (text: string) level ->
        let escapedText = Regex.Replace(text.ToLower(), @"[^\w]+", "-")
        sprintf """<h%s><a name="%s" class="anchor" href="#%s">%s</a></h%s>"""
            level escapedText escapedText text level

    renderer?link <- fun href title text ->
        let href =
            if isAbsoluteUrl href then href
            else Regex.Replace(href, @"\.md$", ".html")
        sprintf """<a href="%s">%s</a>""" href text

    let parseMarkdown(content: string): string =
        marked $ (content, createObj ["renderer" ==> renderer])

let parse (str: string) =
    Util.parseMarkdown str

let parseAsReactEl className (content: string) =
    div [
      if System.String.IsNullOrWhiteSpace(className) |> not then
        yield Class className
      yield DangerouslySetInnerHTML { __html = parse content }
    ] []
