/**
 * relation-graph.js - 关系图谱展示模块
 * 
 * 使用D3.js的力导向图（d3.forceSimulation）将家谱成员以网络图形式展示。
 * 力导向图通过物理模拟算法自动布局节点位置，
 * 节点之间的连线表示父子关系，节点可拖拽调整位置。
 * 
 * 功能特点：
 * - 力导向自动布局，节点位置自动优化
 * - 支持节点拖拽交互
 * - 点击节点显示详细信息侧边栏
 * - 支持缩放和平移操作
 * - 支持全屏展示
 */
(function () {
    'use strict';

    var RelationGraph = {
        svg: null,
        g: null,
        zoom: null,
        simulation: null,
        width: 0,
        height: 0,

        /**
         * 初始化关系图谱
         * 
         * 检查容器和数据是否可用，初始化SVG画布、缩放行为，
         * 然后渲染力导向图并绑定交互事件。
         */
        init: function () {
            var container = document.getElementById('graph-container');
            if (!container || !window.membersData || window.membersData.length === 0) {
                ChartUtils.showEmpty('graph-container');
                return;
            }

            this.width = container.clientWidth;
            this.height = container.clientHeight || 600;

            this.svg = d3.select('#relation-graph')
                .attr('width', this.width)
                .attr('height', this.height);

            this.svg.selectAll('*').remove();

            this.g = this.svg.append('g');

            this.zoom = ChartUtils.initZoom(this.svg, this.g, ChartUtils.updateZoomLevel.bind(ChartUtils));
            this.render();
            this.bindEvents();
        },

        /**
         * 构建力导向图所需的节点和连线数据
         * 
         * 将扁平的成员列表转换为力导向图所需的格式：
         * - nodes: 包含id、姓名、世代、生死状态的节点数组
         * - links: 包含source/target的连线数组，表示父子关系
         * 
         * @param {Array} members - 成员数据数组
         * @returns {Object} 包含 nodes 和 links 的数据对象
         */
        buildGraphData: function (members) {
            var nodes = [];
            var links = [];
            var nodeMap = {};

            members.forEach(function (m) {
                var node = {
                    id: m.id,
                    name: m.fullName,
                    generation: m.generation || 1,
                    isDeceased: m.isDeceased || false
                };
                nodes.push(node);
                nodeMap[m.id] = node;
            });

            members.forEach(function (m) {
                if (m.parentId && nodeMap[m.parentId]) {
                    links.push({
                        source: m.parentId,
                        target: m.id,
                        type: 'parent-child'
                    });
                }
            });

            return { nodes: nodes, links: links };
        },

        /**
         * 渲染力导向图
         * 
         * 创建D3力模拟器，配置各种力（连线力、排斥力、中心力、碰撞力），
         * 然后绘制节点（圆形+文字）和连线，并设置拖拽交互。
         */
        render: function () {
            var data = this.buildGraphData(window.membersData);
            var self = this;

            this.simulation = d3.forceSimulation(data.nodes)
                .force('link', d3.forceLink(data.links).id(function (d) { return d.id; }).distance(150))
                .force('charge', d3.forceManyBody().strength(-500))
                .force('center', d3.forceCenter(this.width / 2, this.height / 2))
                .force('collision', d3.forceCollide().radius(60));

            var link = this.g.append('g')
                .attr('class', 'links')
                .selectAll('line')
                .data(data.links)
                .enter()
                .append('line')
                .attr('class', 'link-line')
                .attr('stroke', '#999')
                .attr('stroke-opacity', 0.6)
                .attr('stroke-width', 2);

            var node = this.g.append('g')
                .attr('class', 'nodes')
                .selectAll('g')
                .data(data.nodes)
                .enter()
                .append('g')
                .attr('class', 'graph-node')
                .attr('data-id', function (d) { return d.id; })
                .call(d3.drag()
                    .on('start', function (event, d) {
                        if (!event.active) self.simulation.alphaTarget(0.3).restart();
                        d.fx = d.x;
                        d.fy = d.y;
                    })
                    .on('drag', function (event, d) {
                        d.fx = event.x;
                        d.fy = event.y;
                    })
                    .on('end', function (event, d) {
                        if (!event.active) self.simulation.alphaTarget(0);
                        d.fx = null;
                        d.fy = null;
                    }));

            node.append('circle')
                .attr('class', function (d) {
                    return 'node-circle' + (d.isDeceased ? ' deceased' : '');
                })
                .attr('r', 30)
                .attr('fill', function (d) {
                    return d.isDeceased ? '#9ca3af' : '#8B4513';
                });

            node.append('text')
                .attr('class', 'node-label')
                .attr('text-anchor', 'middle')
                .attr('dy', '0.35em')
                .attr('fill', 'white')
                .attr('font-size', '12px')
                .text(function (d) {
                    return d.name.length > 4 ? d.name.substring(0, 4) + '...' : d.name;
                });

            node.append('title')
                .text(function (d) {
                    return d.name + ' (第' + d.generation + '世)';
                });

            this.simulation.on('tick', function () {
                link
                    .attr('x1', function (d) { return d.source.x; })
                    .attr('y1', function (d) { return d.source.y; })
                    .attr('x2', function (d) { return d.target.x; })
                    .attr('y2', function (d) { return d.target.y; });

                node.attr('transform', function (d) {
                    return 'translate(' + d.x + ',' + d.y + ')';
                });
            });
        },

        /**
         * 绑定交互事件
         * 
         * 包括缩放控制、全屏切换、节点点击显示侧边栏信息、关闭侧边栏。
         */
        bindEvents: function () {
            var self = this;

            ChartUtils.bindZoomControls(this.svg, this.zoom);
            ChartUtils.bindFullscreen('graph-container');

            this.g.selectAll('.graph-node').on('click', function (event, d) {
                event.stopPropagation();
                self.showNodeInfo(d);
            });

            document.getElementById('close-sidebar')?.addEventListener('click', function () {
                document.getElementById('graph-sidebar').classList.remove('active');
            });
        },

        /**
         * 在侧边栏中显示节点详细信息
         * 
         * 点击图谱节点后，右侧滑出侧边栏，展示成员的姓名、世代、状态等信息，
         * 并提供"查看详情"按钮跳转到完整的成员详情页。
         * 
         * @param {Object} node - 被点击的节点数据对象
         */
        showNodeInfo: function (node) {
            var sidebar = document.getElementById('graph-sidebar');
            var content = document.getElementById('sidebar-content');

            if (sidebar && content) {
                content.innerHTML = ''
                    + '<div class="node-info">'
                    + '  <div class="info-avatar">'
                    + '    <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="currentColor" viewBox="0 0 16 16">'
                    + '      <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>'
                    + '    </svg>'
                    + '  </div>'
                    + '  <h4 class="info-name">' + node.name + '</h4>'
                    + '  <p class="info-generation">第' + node.generation + '世</p>'
                    + '  <p class="info-status">' + (node.isDeceased ? '已故' : '在世') + '</p>'
                    + '  <button class="btn-view-detail" data-id="' + node.id + '">查看详情</button>'
                    + '</div>';

                sidebar.classList.add('active');

                content.querySelector('.btn-view-detail')?.addEventListener('click', function () {
                    var id = this.getAttribute('data-id');
                    ChartUtils.showMemberDetail(id);
                });
            }
        }
    };

    window.RelationGraph = RelationGraph;

    ChartUtils.bootstrapChart('graph-container', 'd3', function () {
        RelationGraph.init();
    });
})();
