/**
 * chart-utils.js - 家谱图谱公共工具模块
 * 
 * 提供所有图表模块共享的工具函数，包括：
 * - 树形数据构建（buildTreeData）
 * - 空状态展示（showEmpty）
 * - D3缩放初始化（initZoom）
 * - 缩放控制事件绑定（bindZoomControls）
 * - 全屏切换（bindFullscreen）
 * - 图表引导启动器（bootstrapChart）
 * - 成员详情查看（showMemberDetail）
 */
(function () {
    'use strict';

    var ChartUtils = {

        /**
         * 将扁平的成员列表构建为树形数据结构
         * 
         * 遍历所有成员，根据 parentId 建立父子关系，
         * 没有父节点的成员作为根节点。
         * 如果存在多个根节点，则创建一个虚拟的"家族"根节点统一管理。
         * 
         * @param {Array} members - 成员数据数组，每个成员需包含 id、parentId、fullName 等字段
         * @param {Object} [options] - 可选配置
         * @param {boolean} [options.returnArray=false] - 是否返回数组形式（用于TreeView等需要多个根的场景）
         * @returns {Object|null} 树形结构的根节点，无数据时返回 null
         */
        buildTreeData: function (members, options) {
            if (!members || members.length === 0) return null;

            options = options || {};

            var memberMap = {};
            var rootMembers = [];

            members.forEach(function (m) {
                memberMap[m.id] = {
                    id: m.id,
                    name: m.fullName,
                    data: m,
                    children: []
                };
            });

            members.forEach(function (m) {
                var node = memberMap[m.id];
                if (m.parentId && memberMap[m.parentId]) {
                    memberMap[m.parentId].children.push(node);
                } else {
                    rootMembers.push(node);
                }
            });

            if (rootMembers.length === 0) return null;

            if (options.returnArray) {
                return rootMembers;
            }

            if (rootMembers.length === 1) return rootMembers[0];

            return {
                id: 'root',
                name: '家族',
                children: rootMembers,
                data: null
            };
        },

        /**
         * 在指定容器中显示空状态提示
         * 
         * 当没有成员数据时，显示一个居中的提示信息，
         * 包含图标、标题和描述文字。
         * 
         * @param {string|HTMLElement} container - 容器元素或其ID
         */
        showEmpty: function (container) {
            var el = typeof container === 'string'
                ? document.getElementById(container)
                : container;
            if (el) {
                el.innerHTML = '<div class="empty-state">'
                    + '<svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="currentColor" viewBox="0 0 16 16">'
                    + '<path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>'
                    + '</svg>'
                    + '<h3>暂无成员数据</h3>'
                    + '<p>请先添加家族成员</p>'
                    + '</div>';
            }
        },

        /**
         * 初始化D3缩放行为
         * 
         * 为SVG元素添加缩放和平移功能，支持鼠标滚轮缩放和拖拽平移。
         * 缩放范围限制在 0.1x 到 3x 之间。
         * 
         * @param {d3.Selection} svg - D3选择的SVG元素
         * @param {d3.Selection} g - D3选择的分组元素（缩放变换的目标）
         * @param {Function} onZoomChange - 缩放比例变化时的回调函数，参数为缩放比例k
         * @returns {d3.ZoomBehavior} D3缩放行为实例
         */
        initZoom: function (svg, g, onZoomChange) {
            var zoom = d3.zoom()
                .scaleExtent([0.1, 3])
                .on('zoom', function (event) {
                    g.attr('transform', event.transform);
                    if (onZoomChange) {
                        onZoomChange(event.transform.k);
                    }
                });

            svg.call(zoom);
            return zoom;
        },

        /**
         * 更新缩放比例显示文本
         * 
         * 将缩放比例转换为百分比格式并更新到指定的DOM元素中。
         * 
         * @param {number} k - 当前缩放比例（1 = 100%）
         */
        updateZoomLevel: function (k) {
            var zoomLevel = document.getElementById('zoom-level');
            if (zoomLevel) {
                zoomLevel.textContent = Math.round(k * 100) + '%';
            }
        },

        /**
         * 绑定缩放控制按钮事件
         * 
         * 为放大、缩小、重置三个按钮绑定点击事件：
         * - 放大：当前比例 × 1.2
         * - 缩小：当前比例 × 0.8
         * - 重置：恢复到初始状态（1:1，居中）
         * 
         * @param {d3.Selection} svg - D3选择的SVG元素
         * @param {d3.ZoomBehavior} zoom - D3缩放行为实例
         */
        bindZoomControls: function (svg, zoom) {
            var self = this;

            document.getElementById('zoom-in')?.addEventListener('click', function () {
                svg.transition().duration(300).call(zoom.scaleBy, 1.2);
            });

            document.getElementById('zoom-out')?.addEventListener('click', function () {
                svg.transition().duration(300).call(zoom.scaleBy, 0.8);
            });

            document.getElementById('zoom-reset')?.addEventListener('click', function () {
                svg.transition().duration(300).call(zoom.transform, d3.zoomIdentity);
            });
        },

        /**
         * 绑定全屏切换按钮事件
         * 
         * 点击按钮时切换指定容器的全屏状态。
         * 如果当前已全屏则退出，否则进入全屏模式。
         * 
         * @param {string} containerId - 需要全屏显示的容器元素ID
         */
        bindFullscreen: function (containerId) {
            document.getElementById('fullscreen-btn')?.addEventListener('click', function () {
                var container = document.getElementById(containerId);
                if (!container) return;
                if (document.fullscreenElement) {
                    document.exitFullscreen();
                } else {
                    container.requestFullscreen();
                }
            });
        },

        /**
         * 图表模块引导启动器
         * 
         * 提供统一的图表初始化入口，解决以下问题：
         * 1. 通过AJAX加载时 DOMContentLoaded 不会再次触发的问题
         * 2. D3库异步加载的等待问题
         * 3. 容器元素是否存在的检查
         * 
         * 使用方式：在图表模块末尾调用此方法，传入容器ID、需要等待的全局变量名、初始化函数。
         * 
         * @param {string} containerId - 图表容器元素的ID
         * @param {string} requiredGlobal - 需要等待的全局变量名（如 'd3'），为空则不等待
         * @param {Function} initFn - 图表初始化函数
         */
        bootstrapChart: function (containerId, requiredGlobal, initFn) {
            function tryInit() {
                if (!document.getElementById(containerId)) {
                    return;
                }

                if (requiredGlobal && typeof window[requiredGlobal] === 'undefined') {
                    setTimeout(tryInit, 50);
                    return;
                }

                initFn();
            }

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', tryInit);
            } else {
                tryInit();
            }
        },

        /**
         * 查看成员详情
         * 
         * 通过FamilyTreeApp全局对象打开成员详情弹窗。
         * 如果FamilyTreeApp不可用则静默失败。
         * 
         * @param {string} memberId - 成员ID
         */
        showMemberDetail: function (memberId) {
            if (memberId && window.FamilyTreeApp) {
                window.FamilyTreeApp.showMemberDetail(memberId);
            }
        }
    };

    window.ChartUtils = ChartUtils;
})();
