/**
 * table-view.js - 成员列表表格模块
 * 
 * 以分页表格形式展示家谱成员的详细信息列表。
 * 支持排序、搜索和导出CSV功能。
 * 
 * 功能特点：
 * - 按世代、姓名、出生日期排序
 * - 导出为CSV文件（UTF-8 BOM编码，兼容Excel中文显示）
 * - 点击查看按钮可查看成员详情
 */
(function () {
    'use strict';

    var TableView = {
        /** 当前排序的列名 */
        sortColumn: null,

        /** 当前排序方向：asc（升序）或 desc（降序） */
        sortDirection: 'asc',

        /**
         * 初始化表格视图
         * 
         * 绑定排序、查看详情和导出等交互事件。
         */
        init: function () {
            this.bindEvents();
        },

        /**
         * 绑定交互事件
         * 
         * 包括：
         * - 表头点击排序
         * - 查看详情按钮点击
         * - 导出Excel按钮点击
         */
        bindEvents: function () {
            var self = this;

            document.querySelectorAll('.sortable').forEach(function (th) {
                th.addEventListener('click', function () {
                    var column = this.getAttribute('data-sort');
                    self.sortTable(column);
                });
            });

            document.querySelectorAll('.view-btn').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var id = this.getAttribute('data-id');
                    ChartUtils.showMemberDetail(id);
                });
            });

            document.getElementById('export-excel')?.addEventListener('click', function () {
                self.exportToExcel();
            });
        },

        /**
         * 对表格进行排序
         * 
         * 点击同一列时切换升序/降序，点击不同列时默认升序。
         * 排序后更新表头样式并重新排列表格行。
         * 
         * @param {string} column - 要排序的列名（generation/name/birthDate）
         */
        sortTable: function (column) {
            var table = document.getElementById('member-table');
            var tbody = table.querySelector('tbody');
            var rows = Array.from(tbody.querySelectorAll('tr:not(.empty-row)'));
            var self = this;

            if (this.sortColumn === column) {
                this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
            } else {
                this.sortColumn = column;
                this.sortDirection = 'asc';
            }

            document.querySelectorAll('.sortable').forEach(function (th) {
                th.classList.remove('sort-asc', 'sort-desc');
            });

            var currentTh = document.querySelector('.sortable[data-sort="' + column + '"]');
            if (currentTh) {
                currentTh.classList.add('sort-' + this.sortDirection);
            }

            rows.sort(function (a, b) {
                var aVal = self.getCellValue(a, column);
                var bVal = self.getCellValue(b, column);

                if (aVal < bVal) return self.sortDirection === 'asc' ? -1 : 1;
                if (aVal > bVal) return self.sortDirection === 'asc' ? 1 : -1;
                return 0;
            });

            rows.forEach(function (row) {
                tbody.appendChild(row);
            });
        },

        /**
         * 获取表格行中指定列的值
         * 
         * 根据列名从表格行中提取对应的值用于排序比较：
         * - generation: 从世代标签中提取数字
         * - name: 获取成员姓名文本
         * - birthDate: 获取出生日期文本
         * 
         * @param {HTMLTableRowElement} row - 表格行元素
         * @param {string} column - 列名
         * @returns {string|number} 用于排序比较的值
         */
        getCellValue: function (row, column) {
            switch (column) {
                case 'generation':
                    var genBadge = row.querySelector('.generation-badge');
                    if (genBadge) {
                        var match = genBadge.textContent.match(/第(\d+)世/);
                        return match ? parseInt(match[1]) : 0;
                    }
                    return 0;
                case 'name':
                    var nameCell = row.querySelector('.member-name');
                    return nameCell ? nameCell.textContent.trim() : '';
                case 'birthDate':
                    var dateCell = row.querySelectorAll('td')[3];
                    return dateCell ? dateCell.textContent.trim() : '';
                default:
                    return '';
            }
        },

        /**
         * 导出表格数据为CSV文件
         * 
         * 将当前表格内容转换为CSV格式并触发下载。
         * 使用UTF-8 BOM编码确保Excel正确显示中文。
         * 文件名包含当前日期以便区分。
         */
        exportToExcel: function () {
            var table = document.getElementById('member-table');
            var rows = table.querySelectorAll('tr');
            var csv = [];

            var headers = [];
            var headerCells = rows[0].querySelectorAll('th');
            headerCells.forEach(function (cell) {
                headers.push(cell.textContent.trim().replace(/\n/g, ''));
            });
            csv.push(headers.join(','));

            for (var i = 1; i < rows.length; i++) {
                var cells = rows[i].querySelectorAll('td');
                var rowData = [];
                cells.forEach(function (cell) {
                    var text = cell.textContent.trim().replace(/,/g, '，').replace(/\n/g, ' ');
                    rowData.push('"' + text + '"');
                });
                csv.push(rowData.join(','));
            }

            var csvContent = '\uFEFF' + csv.join('\n');
            var blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            var link = document.createElement('a');
            var url = URL.createObjectURL(blob);

            link.setAttribute('href', url);
            link.setAttribute('download', 'family_members_' + new Date().toISOString().slice(0, 10) + '.csv');
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    };

    window.TableView = TableView;

    ChartUtils.bootstrapChart('member-table', null, function () {
        TableView.init();
    });
})();
