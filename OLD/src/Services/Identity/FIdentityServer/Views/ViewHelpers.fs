module ViewHelpers

open Giraffe
open GiraffeViewEngine

let _dataToggle = attr "data-toggle"
let _dataTarget = attr "data-target"
let _dataUrl = attr "data-url"
let _ariaExpanded = attr "aria-expanded"

let iframe = tag "iframe"
