/**
 * tree-view.js - 树形结构展示模块
 * 
 * 以可折叠的树形列表形式展示家谱成员的层级关系。
 * 每个成员节点显示姓名和世代信息，有子节点的成员可以展开/折叠。
 * 
 * 功能特点：
 * - 支持展开/折叠子节点
 * - 支持全部展开/全部折叠
 * - 支持按姓名搜索成员
 * - 搜索时自动展开匹配节点的父级路径
 * - 点击节点可查看成员详情
 */
(function () {
    'use strict';

    var TreeView = {
        /** 记录每个节点的展开状态，key为节点id，value为布尔值 */
        expandedNodes: {},

        /**
         * 初始化树形视图
         * 
         * 检查数据是否可用，计算统计信息，渲染树形结构并绑定交互事件。
         */
        init: function () {
            if (!window.membersData || window.membersData.length === 0) {
                ChartUtils.showEmpty('tree-container');
                return;
            }

            this.calculateStats();
            this.render();
            this.bindEvents();
        },

        /**
         * 计算并显示统计信息
         * 
         * 统计总成员数和最大世代数，更新页面上的统计显示元素。
         */
        calculateStats: function () {
            var members = window.membersData;
            var totalMembers = members.length;
            var maxGeneration = 0;

            members.forEach(function (m) {
                if (m.generation && m.generation > maxGeneration) {
                    maxGeneration = m.generation;
                }
            });

            var totalMembersEl = document.getElementById('total-members');
            var totalGenEl = document.getElementById('total-generations');
            if (totalMembersEl) totalMembersEl.textContent = totalMembers;
            if (totalGenEl) totalGenEl.textContent = maxGeneration;
        },

        /**
         * 渲染树形结构
         * 
         * 使用公共工具构建树形数据，然后递归渲染每个根节点及其子节点。
         */
        render: function () {
            var rootMembers = ChartUtils.buildTreeData(window.membersData, { returnArray: true });
            var container = document.getElementById('tree-root');

            if (!container || !rootMembers) return;

            var html = '';
            var self = this;

            rootMembers.forEach(function (root) {
                html += self.renderNode(root, 0);
            });

            container.innerHTML = html;
        },

        /**
         * 递归渲染单个树节点
         * 
         * 为每个节点生成HTML，包含：
         * - 展开/折叠按钮（有子节点时）
         * - 成员图标（区分已故/在世）
         * - 成员姓名
         * - 世代标签
         * - 子节点列表（递归渲染）
         * 
         * @param {Object} node - 树节点数据对象
         * @param {number} level - 当前节点的层级深度，用于计算缩进
         * @returns {string} 节点的HTML字符串
         */
        renderNode: function (node, level) {
            var hasChildren = node.children && node.children.length > 0;
            var isExpanded = this.expandedNodes[node.id] !== false;
            var self = this;

            var html = '<li class="tree-node' + (hasChildren ? ' has-children' : '') + '" data-id="' + node.id + '">';

            html += '<div class="node-content" style="padding-left: ' + (level * 24) + 'px;">';

            if (hasChildren) {
                html += '<button class="toggle-btn' + (isExpanded ? ' expanded' : '') + '">';
                html += '<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" fill="currentColor" viewBox="0 0 16 16">';
                html += '<path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/>';
                html += '</svg>';
                html += '</button>';
            } else {
                html += '<span class="toggle-placeholder"></span>';
            }

            html += '<span class="node-icon' + (node.data && node.data.isDeceased ? ' deceased' : '') + '">';
            html += '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">';
            html += '<path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>';
            html += '</svg>';
            html += '</span>';

            html += '<span class="node-name">' + (node.name || '未知') + '</span>';

            if (node.data && node.data.generation) {
                html += '<span class="node-badge">第' + node.data.generation + '世</span>';
            }

            html += '</div>';

            if (hasChildren) {
                html += '<ul class="tree-children' + (isExpanded ? '' : ' collapsed') + '">';
                node.children.forEach(function (child) {
                    html += self.renderNode(child, level + 1);
                });
                html += '</ul>';
            }

            html += '</li>';
            return html;
        },

        /**
         * 绑定交互事件
         * 
         * 包括：
         * - 展开/折叠按钮点击
         * - 节点内容点击查看详情
         * - 全部展开/折叠按钮
         * - 搜索框输入
         */
        bindEvents: function () {
            var self = this;

            document.querySelectorAll('.tree-node .toggle-btn').forEach(function (btn) {
                btn.addEventListener('click', function (e) {
                    e.stopPropagation();
                    var node = this.closest('.tree-node');
                    var children = node.querySelector(':scope > .tree-children');
                    var id = node.getAttribute('data-id');

                    if (children) {
                        if (children.classList.contains('collapsed')) {
                            children.classList.remove('collapsed');
                            this.classList.add('expanded');
                            self.expandedNodes[id] = true;
                        } else {
                            children.classList.add('collapsed');
                            this.classList.remove('expanded');
                            self.expandedNodes[id] = false;
                        }
                    }
                });
            });

            document.querySelectorAll('.tree-node .node-content').forEach(function (content) {
                content.addEventListener('click', function (e) {
                    if (e.target.closest('.toggle-btn')) return;

                    var node = this.closest('.tree-node');
                    var id = node.getAttribute('data-id');
                    ChartUtils.showMemberDetail(id);
                });
            });

            document.getElementById('expand-all')?.addEventListener('click', function () {
                document.querySelectorAll('.tree-children').forEach(function (ul) {
                    ul.classList.remove('collapsed');
                });
                document.querySelectorAll('.tree-node .toggle-btn').forEach(function (btn) {
                    btn.classList.add('expanded');
                });
                Object.keys(self.expandedNodes).forEach(function (key) {
                    self.expandedNodes[key] = true;
                });
            });

            document.getElementById('collapse-all')?.addEventListener('click', function () {
                document.querySelectorAll('.tree-children').forEach(function (ul) {
                    ul.classList.add('collapsed');
                });
                document.querySelectorAll('.tree-node .toggle-btn').forEach(function (btn) {
                    btn.classList.remove('expanded');
                });
                Object.keys(self.expandedNodes).forEach(function (key) {
                    self.expandedNodes[key] = false;
                });
            });

            document.getElementById('tree-search')?.addEventListener('input', function () {
                var keyword = this.value.toLowerCase().trim();
                self.searchNodes(keyword);
            });
        },

        /**
         * 搜索节点
         * 
         * 根据关键词搜索成员姓名：
         * - 匹配的节点高亮显示
         * - 不匹配的节点隐藏
         * - 匹配节点的所有父级节点自动展开并显示
         * - 清空关键词时恢复所有节点的显示状态
         * 
         * @param {string} keyword - 搜索关键词（小写）
         */
        searchNodes: function (keyword) {
            var nodes = document.querySelectorAll('.tree-node');

            if (!keyword) {
                nodes.forEach(function (node) {
                    node.classList.remove('hidden', 'highlight');
                });
                return;
            }

            nodes.forEach(function (node) {
                var name = node.querySelector('.node-name').textContent.toLowerCase();
                if (name.includes(keyword)) {
                    node.classList.remove('hidden');
                    node.classList.add('highlight');
                    var parent = node.parentElement.closest('.tree-node');
                    while (parent) {
                        parent.classList.remove('hidden');
                        var children = parent.querySelector(':scope > .tree-children');
                        if (children) {
                            children.classList.remove('collapsed');
                        }
                        var toggleBtn = parent.querySelector(':scope > .node-content > .toggle-btn');
                        if (toggleBtn) {
                            toggleBtn.classList.add('expanded');
                        }
                        parent = parent.parentElement.closest('.tree-node');
                    }
                } else {
                    node.classList.add('hidden');
                    node.classList.remove('highlight');
                }
            });
        }
    };

    window.TreeView = TreeView;

    ChartUtils.bootstrapChart('tree-container', null, function () {
        TreeView.init();
    });
})();
