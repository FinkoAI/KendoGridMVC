/*beg = Batch Editor Grid*/
var s, beg = {
    settings: {
        notification: null,
        batchEditor: null,
        grid: null,
        isBatchEditorOn: false,
        checkedIds: {}
    },

    init: function (pgridid, notifId, pexternalEditorid) {
        s = this.settings;
        s.notification = $('#' + notifId).kendoNotification({
            position: {
                top: 30,
                right: 30
            },
            stacking: "down",
            templates: [{
                type: "error",
                template: $("#errorTemplate").html()
            }]
        }).data("kendoNotification");
        s.grid = $('#' + pgridid).data().kendoGrid;
        s.batchEditor = $('#' + pexternalEditorid);       
    },
    rowSelection: function (chkbx) {
        if (chkbx != null) {
            var row = chkbx.closest("tr");
            var dataItem = s.grid.dataItem(row);
            beg.AddCheckedid(dataItem.id, chkbx.checked);
            if (chkbx.checked) {
                row.className = "k-state-selected";
            } else {
                row.className = "k-state-selected";
            }
        }
    },
    showCheckBoxes: function () {
        s.isBatchEditorOn = true;
        if (s.grid != null)
            s.grid.showColumn(0);
    },
    changeApply: function () {
        var columns = s.grid.options.columns;
        var data = s.grid._data;
        for (var i in s.checkedIds) {
            for (var j = 0; j < columns.length; j++) {
                if (columns[j].encoded) {
                    var proprtyField = columns[j].field.replace('.', '_');

                    // drop down case
                    if (columns[j].title === 'Product Category') {
                        var dropDown = s.batchEditor.find("#" + proprtyField).data("kendoDropDownList");
                        var record = dropDown.dataItem();
                        if (s.checkedIds[i] && record != "" && record.ProductCategoryId > 0) {
                            var dt = data[i - 1];
                            dt["ProductCategory"].ProductCategoryId = record.ProductCategoryId;
                            dt["ProductCategory"].ProductCategoryName = record.ProductCategoryName;
                            dt.dirty = true;
                            s.grid._modelChange({ model: dt });
                        }

                    } else {
                        // input field case
                        var obj = s.batchEditor.find("input[name=" + proprtyField + "]");
                        var record = obj.val();
                        if (s.checkedIds[i] && record != "") {
                            var dt = data[i - 1];
                            dt[proprtyField] = record;
                            dt.dirty = true;
                            s.grid._modelChange({ model: dt });
                        }
                    }
                }
            }
        }
        // Clear Controls
        for (var j = 0; j < columns.length; j++) {
            if (columns[j].encoded) {
                var proprtyField = columns[j].field.replace('.', '_');
                if (columns[j].title === 'Product Category') {
                    var dropDown = s.batchEditor.find("#" + proprtyField).data("kendoDropDownList");
                    dropDown.select(0);
                } else {
                    var obj = s.batchEditor.find("input[name=" + proprtyField + "]");
                    obj.val("");
                }
            }
        }
        s.checkedIds = {};
    },

    createEditor: function () {
        var columns = s.grid.options.columns;
        var table = $('<table></table>').addClass('k-widget k-dialog k-window');
        for (var j = 0; j < columns.length; j++) {
            if (columns[j].encoded) {
                var clmn = columns[j];
                var row = $('<tr></tr>').addClass('k-content');
                var field = clmn.field.replace('.', '_');
                var rowData1 = $('<td></td>').text(clmn.title);
                var rowData2 = $('<td></td>');
                rowData2.html(clmn.editor);
                row.append(rowData1);
                row.append(rowData2);
                table.append(row);
            }
        }
        var row = $('<tr></tr>');
        var rowDataBlank = $('<td></td>');
        row.append(rowDataBlank);
        var rowDataBtn = $('<td style="float:right;"></td>');
        rowDataBtn.html('<button id="btnApply" type="k-button" data-role="button" class="k-button k-button-icontext" role="button" aria-disabled="false" tabindex="0"><span class="k-icon k-i-settings"></span>Apply</button>');
        row.append(rowDataBtn);
        table.append(row);
        s.batchEditor.html(table);
    },
    Open: function () {
        if (s === null || s === undefined)
            return false;

        // Show Checkbox Column 
        if (!s.isBatchEditorOn) {
            beg.showCheckBoxes();
            return false;
        }
        // Check At Least One Selected
        var checkedRow = [];
        for (var i in s.checkedIds) {
            if (s.checkedIds[i]) {
                checkedRow.push(i);
            }
        }

        if (checkedRow.length === 0) {

            s.notification.show({
                title: "Batch Editor Message",
                message: "Please select at least one row to batch-edit"
            }, "error");
            return false;
        }

        // Create Batch Editor Once.
        if (s.batchEditor.html().length === 1)
            beg.createEditor();

        kendo.bind(s.batchEditor);
        $("#btnApply").kendoButton({ "click": beg.changeApply, "icon": "settings" });
        var wdw = s.batchEditor.data('kendoWindow').open();
        wdw.title("Batch Editor");
        wdw.setOptions({
            width: 340,
            height: 175
        });
        wdw.center();
        wdw.open();  //and call its open method
    },
    AddCheckedid: function (pid, value) {
        s.checkedIds[pid] = value;
    },
    ReadComplete: function () {
        if (s === null || s === undefined)
            return false;
        s.isBatchEditorOn = false;
        if (s.grid != null)
            s.grid.hideColumn(0);
    }
};