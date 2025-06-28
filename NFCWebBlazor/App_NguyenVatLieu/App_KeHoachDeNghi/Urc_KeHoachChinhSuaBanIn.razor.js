var gridTbodySelector = "#griddragdrop .dxbl-grid .dxbl-grid-table > tbody";

var dotNetHelper;
function setDotNetHelper(helper) {
    console.log("setDotNetHelper test thử");
    dotNetHelper = helper;
}
function initialize() {
    var draggableElementContainer = createDraggableElementContainer();
    var draggableElementTable = draggableElementContainer.querySelector("table");
    var draggableElementTBody = draggableElementContainer.querySelector("tbody");
    console.log("initialize test thử");
    $(function () {
        
        if ($(gridTbodySelector).length === 0)
            return;
        $(gridTbodySelector).sortable({
            items: "tr[data-visible-index]",
            cursor: 'move',
            helper: "clone",
            appendTo: draggableElementTBody,
            placeholder: "ui-state-highlight",

            start: function (e, ui) {
                var originalTable = ui.item[0].parentNode.parentNode;

                draggableElementTable.className = originalTable.className;
                draggableElementTable.style.width = originalTable.offsetWidth + "px";

                var cols = originalTable.querySelectorAll(":scope > colgroup > col");
                var row = ui.helper[0];
                for (var i = 0; i < cols.length; i++) {
                   
                    row.cells[i].style.width = cols[i].offsetWidth + "px";
                }

                row.style.backgroundColor = "white";
                
            },
            stop: function (e, ui) {
                var row = ui.item[0];
                console.log("log stop");
                var prevRow = row.previousElementSibling;
                var nextRow = row.nextElementSibling;
                 
                //console.log(getVisibleIndex(row));
                //console.log(getVisibleIndex(prevRow));
                //console.log(getVisibleIndex(nextRow));

                //console.log(prevRow.dataset.visibleIndex);

                //console.log(nextRow.dataset.visibleIndex);
                //Kiểm tra các trường hợp dòng ở đầu và ở cuối khi move nó bị undefined
                var nextindex = 0;
                var previndex = 0;
                if (typeof prevRow.dataset.visibleIndex !== 'undefined' && typeof nextRow.dataset.visibleIndex !== 'undefined')
                {
                    previndex = getVisibleIndex(prevRow);
                    nextindex = getVisibleIndex(nextRow);
                }
                if (typeof prevRow.dataset.visibleIndex === 'undefined' && typeof nextRow.dataset.visibleIndex !== 'undefined') {
                    nextindex = getVisibleIndex(nextRow);
                    if (nextindex > 0)
                        previndex = nextindex - 1;
                    else
                        previndex = 0;
                    //console.log(" thang prevRow no null ");
                }
                if (typeof prevRow.dataset.visibleIndex !== 'undefined' && typeof nextRow.dataset.visibleIndex === 'undefined')
                {
                    previndex = getVisibleIndex(prevRow);
                    nextindex = previndex;
                    if (previndex - 1 >= 0)
                        previndex = previndex - 1;
                   
                }

              
               
                window.setTimeout(async function () {
                    console.log("ReorderGridRows");

                    await dotNetHelper.invokeMethodAsync("ReorderGridRows", getVisibleIndex(row), previndex, nextindex);
                }, 50);
            }
        });
    });
}
function getVisibleIndex(row) {
    var visibleIndex = -1;
   
    if (row && Object.keys(row.dataset).length > 0)
        visibleIndex = parseInt(row.dataset.visibleIndex);
    return visibleIndex;
}
function createDraggableElementContainer() {
    var container = document.createElement("DIV");
    container.innerHTML = "<table style='position: absolute; left: -10000px; top: -10000px;'><tbody></tbody></table>";
    document.body.appendChild(container);
    return container;
}

export { setDotNetHelper, initialize }