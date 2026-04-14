/**
 * table-chart.js - 世系表格展示模块
 * 
 * 按世代分组以表格形式展示家谱成员。
 * 每一世代为一行，展示该世代的所有成员。
 * 支持横向和纵向两种展示方向。
 * 
 * 功能特点：
 * - 按世代分组展示成员
 * - 支持横向/纵向方向切换
 * - 点击成员可查看详情
 * - 支持导出功能
 */
(function () {
    'use strict';

    var TableChart = {
        /** 当前展示方向：horizontal（横向）或 vertical（纵向） */
        direction: 'horizontal',

        /**
         * 初始化世系表格
         * 
         * 读取方向配置，检查数据是否可用，渲染表格并绑定交互事件。
         */
        init: function () {
            this.direction = window.direction || 'horizontal';

            if (!window.membersData || window.membersData.length === 0) {
                ChartUtils.showEmpty('table-chart-container');
                return;
            }

            this.render();
            this.bindEvents();
        },

        /**
         * 渲染世系表格
         * 
         * 将成员按世代分组，然后分别渲染世代标签和成员内容区域。
         */
        render: function () {
            var members = window.membersData;
            var generations = this.groupByGeneration(members);

            this.renderGenerationLabels(generations);
            this.renderTableContent(generations);
        },

        /**
         * 按世代对成员进行分组
         * 
         * 遍历所有成员，根据 generation 字段分组，
         * 然后按世代从小到大排序。
         * 
         * @param {Array} members - 成员数据数组
         * @returns {Array} 排序后的世代数组，每项包含 generation（世代数）和 members（成员数组）
         */
        groupByGeneration: function (members) {
            var generations = {};

            members.forEach(function (m) {
                var gen = m.generation || 1;
                if (!generations[gen]) {
                    generations[gen] = [];
                }
                generations[gen].push(m);
            });

            var sortedGenerations = [];
            Object.keys(generations).sort(function (a, b) {
                return parseInt(a) - parseInt(b);
            }).forEach(function (key) {
                sortedGenerations.push({
                    generation: parseInt(key),
                    members: generations[key]
                });
            });

            return sortedGenerations;
        },

        /**
         * 渲染世代标签区域
         * 
         * 在表格左侧（横向模式）或顶部（纵向模式）显示世代标签，
         * 如"第1世"、"第2世"等。
         * 
         * @param {Array} generations - 排序后的世代数组
         */
        renderGenerationLabels: function (generations) {
            var container = document.getElementById('generation-labels');
            if (!container) return;

            var html = '';
            generations.forEach(function (gen) {
                html += '<div class="generation-label">第' + gen.generation + '世</div>';
            });

            container.innerHTML = html;
        },

        /**
         * 渲染表格内容区域
         * 
         * 按世代分行展示成员，每个成员显示为一个可点击的卡片，
         * 包含姓名和字辈信息，已故成员有特殊样式。
         * 
         * @param {Array} generations - 排序后的世代数组
         */
        renderTableContent: function (generations) {
            var container = document.getElementById('table-chart-content');
            if (!container) return;

            var html = '<div class="table-chart-grid">';

            generations.forEach(function (gen) {
                html += '<div class="generation-row">';
                html += '<div class="generation-members">';

                gen.members.forEach(function (member) {
                    html += '<div class="member-cell" data-id="' + member.id + '">';
                    html += '<div class="member-cell-content' + (member.isDeceased ? ' deceased' : '') + '">';
                    html += '<span class="member-name">' + (member.fullName || '未知') + '</span>';
                    if (member.generationName) {
                        html += '<span class="member-gen-name">' + member.generationName + '</span>';
                    }
                    html += '</div>';
                    html += '</div>';
                });

                html += '</div>';
                html += '</div>';
            });

            html += '</div>';
            container.innerHTML = html;
        },

        /**
         * 绑定交互事件
         * 
         * 包括方向切换按钮和成员卡片点击查看详情。
         */
        bindEvents: function () {
            document.querySelectorAll('.toggle-btn[data-direction]').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var direction = this.getAttribute('data-direction');
                    var familyTreeId = window.familyTreeId;
                    window.location.href = '/FamilyTree/TableChart/' + familyTreeId + '?direction=' + direction;
                });
            });

            document.querySelectorAll('.member-cell').forEach(function (cell) {
                cell.addEventListener('click', function () {
                    var id = this.getAttribute('data-id');
                    ChartUtils.showMemberDetail(id);
                });
            });
        }
    };

    window.TableChart = TableChart;

    ChartUtils.bootstrapChart('table-chart-container', null, function () {
        TableChart.init();
    });
})();
