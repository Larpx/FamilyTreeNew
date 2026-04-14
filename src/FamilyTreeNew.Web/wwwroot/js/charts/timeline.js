/**
 * timeline.js - 时间轴展示模块
 * 
 * 以时间轴形式展示家谱成员的出生和逝世事件。
 * 事件按时间顺序排列，交替显示在时间轴两侧。
 * 
 * 功能特点：
 * - 按时间顺序展示出生/逝世事件
 * - 支持按事件类型筛选（全部/出生/逝世）
 * - 统计出生事件数、逝世事件数和总事件数
 * - 点击事件可查看成员详情
 */
(function () {
    'use strict';

    var Timeline = {
        /** 所有事件数据数组 */
        events: [],

        /** 当前筛选条件：all（全部）、birth（出生）、death（逝世） */
        filter: 'all',

        /**
         * 初始化时间轴
         * 
         * 检查数据是否可用，构建事件数据，计算统计信息，
         * 渲染时间轴并绑定交互事件。
         */
        init: function () {
            if (!window.membersData || window.membersData.length === 0) {
                ChartUtils.showEmpty('timeline-container');
                return;
            }

            this.buildEvents();
            this.calculateStats();
            this.render();
            this.bindEvents();
        },

        /**
         * 从成员数据构建事件列表
         * 
         * 遍历所有成员，为每个有出生日期的成员创建出生事件，
         * 为每个已故且有逝世日期的成员创建逝世事件。
         * 事件按日期从早到晚排序。
         */
        buildEvents: function () {
            var self = this;
            this.events = [];

            window.membersData.forEach(function (member) {
                if (member.birthDateSolar) {
                    self.events.push({
                        type: 'birth',
                        date: new Date(member.birthDateSolar),
                        year: new Date(member.birthDateSolar).getFullYear(),
                        member: member,
                        title: member.fullName + ' 出生',
                        description: '第' + (member.generation || 1) + '世'
                    });
                }

                if (member.isDeceased && member.deathDateSolar) {
                    self.events.push({
                        type: 'death',
                        date: new Date(member.deathDateSolar),
                        year: new Date(member.deathDateSolar).getFullYear(),
                        member: member,
                        title: member.fullName + ' 逝世',
                        description: '享年约 ' + self.calculateAge(member) + ' 岁'
                    });
                }
            });

            this.events.sort(function (a, b) {
                return a.date - b.date;
            });
        },

        /**
         * 计算成员享年
         * 
         * 根据出生日期和逝世日期计算年龄（取整）。
         * 
         * @param {Object} member - 成员数据对象
         * @returns {number} 享年年龄，缺少日期时返回0
         */
        calculateAge: function (member) {
            if (!member.birthDateSolar || !member.deathDateSolar) return 0;
            var birth = new Date(member.birthDateSolar);
            var death = new Date(member.deathDateSolar);
            return Math.floor((death - birth) / (365.25 * 24 * 60 * 60 * 1000));
        },

        /**
         * 计算并更新统计信息
         * 
         * 统计出生事件数、逝世事件数和总事件数，
         * 更新页面上的统计显示元素。
         */
        calculateStats: function () {
            var birthCount = this.events.filter(function (e) { return e.type === 'birth'; }).length;
            var deathCount = this.events.filter(function (e) { return e.type === 'death'; }).length;

            var birthEl = document.getElementById('birth-count');
            var deathEl = document.getElementById('death-count');
            var totalEl = document.getElementById('total-events');
            if (birthEl) birthEl.textContent = birthCount;
            if (deathEl) deathEl.textContent = deathCount;
            if (totalEl) totalEl.textContent = this.events.length;
        },

        /**
         * 渲染时间轴
         * 
         * 根据当前筛选条件过滤事件，然后将事件交替排列在时间轴左右两侧。
         * 每个事件项包含日期、图标、标题和描述信息。
         * 修复了原代码中 self 引用错误，改用 this.filter。
         */
        render: function () {
            var container = document.getElementById('timeline-items');
            if (!container) return;

            var self = this;
            var filteredEvents = this.events;
            if (this.filter !== 'all') {
                filteredEvents = this.events.filter(function (e) {
                    return e.type === self.filter;
                });
            }

            if (filteredEvents.length === 0) {
                container.innerHTML = '<div class="empty-message">暂无事件数据</div>';
                return;
            }

            var html = '';
            filteredEvents.forEach(function (event, index) {
                var side = index % 2 === 0 ? 'left' : 'right';
                var iconClass = event.type === 'birth' ? 'birth' : 'death';

                html += '<div class="timeline-item ' + side + '" data-id="' + event.member.id + '">';
                html += '<div class="timeline-marker ' + iconClass + '"></div>';
                html += '<div class="timeline-content">';
                html += '<div class="timeline-date">' + event.date.toLocaleDateString('zh-CN') + '</div>';
                html += '<div class="timeline-icon ' + iconClass + '">';

                if (event.type === 'birth') {
                    html += '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">';
                    html += '<path d="M8 4.754a3.246 3.246 0 1 0 0 6.492 3.246 3.246 0 0 0 0-6.492zM5.754 8a2.246 2.246 0 1 1 4.492 0 2.246 2.246 0 0 1-4.492 0z"/>';
                    html += '</svg>';
                } else {
                    html += '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">';
                    html += '<path d="M8 4.754a3.246 3.246 0 1 0 0 6.492 3.246 3.246 0 0 0 0-6.492z"/>';
                    html += '</svg>';
                }

                html += '</div>';
                html += '<h4 class="timeline-title">' + event.title + '</h4>';
                html += '<p class="timeline-desc">' + event.description + '</p>';
                html += '</div>';
                html += '</div>';
            });

            container.innerHTML = html;
        },

        /**
         * 绑定交互事件
         * 
         * 包括事件类型筛选下拉框和事件项点击查看详情。
         * 筛选变化时重新渲染时间轴。
         */
        bindEvents: function () {
            var self = this;

            document.getElementById('event-filter')?.addEventListener('change', function () {
                self.filter = this.value;
                self.render();
            });

            document.querySelectorAll('.timeline-item').forEach(function (item) {
                item.addEventListener('click', function () {
                    var id = this.getAttribute('data-id');
                    ChartUtils.showMemberDetail(id);
                });
            });
        }
    };

    window.Timeline = Timeline;

    ChartUtils.bootstrapChart('timeline-container', null, function () {
        Timeline.init();
    });
})();
