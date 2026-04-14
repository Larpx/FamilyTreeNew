/**
 * lineage-chart.js - 吊线图展示模块
 * 
 * 使用D3.js的树形布局（d3.tree）将家谱成员以吊线图形式展示。
 * 吊线图是一种传统的家谱展示方式，从上到下按世代排列，
 * 父子关系通过连线表示，形似悬挂的线段。
 * 
 * 功能特点：
 * - 支持缩放和平移操作
 * - 区分已故/在世成员的显示样式
 * - 点击节点可查看成员详情
 * - 支持全屏展示
 */
(function () {
    'use strict';

    var LineageChart = {
        svg: null,
        g: null,
        zoom: null,
        width: 0,
        height: 0,

        nodeWidth: 120,
        nodeHeight: 60,
        horizontalSpacing: 40,
        verticalSpacing: 80,

        /**
         * 初始化吊线图
         * 
         * 检查容器和数据是否可用，初始化SVG画布、缩放行为，
         * 然后渲染图表并绑定交互事件。
         */
        init: function () {
            var container = document.getElementById('lineage-chart-container');
            if (!container || !window.membersData || window.membersData.length === 0) {
                ChartUtils.showEmpty('lineage-chart-container');
                return;
            }

            this.width = container.clientWidth;
            this.height = container.clientHeight || 600;

            this.svg = d3.select('#lineage-chart')
                .attr('width', this.width)
                .attr('height', this.height);

            this.svg.selectAll('*').remove();

            this.g = this.svg.append('g');

            this.zoom = ChartUtils.initZoom(this.svg, this.g, ChartUtils.updateZoomLevel.bind(ChartUtils));
            this.render();
            this.bindEvents();
        },

        /**
         * 渲染吊线图
         * 
         * 使用D3树形布局算法计算节点位置，
         * 然后绘制连线和节点矩形，并在节点中显示姓名和世代信息。
         * 自动计算偏移量使图表居中显示。
         */
        render: function () {
            var treeData = ChartUtils.buildTreeData(window.membersData);
            if (!treeData) {
                ChartUtils.showEmpty('lineage-chart-container');
                return;
            }

            var treeLayout = d3.tree()
                .nodeSize([this.nodeWidth + this.horizontalSpacing, this.nodeHeight + this.verticalSpacing]);

            var root = d3.hierarchy(treeData);
            treeLayout(root);

            var nodes = root.descendants();
            var links = root.links();

            var minX = d3.min(nodes, function (d) { return d.x; });
            var maxX = d3.max(nodes, function (d) { return d.x; });
            var minY = d3.min(nodes, function (d) { return d.y; });
            var maxY = d3.max(nodes, function (d) { return d.y; });

            var treeWidth = maxX - minX + this.nodeWidth * 2;
            var treeHeight = maxY - minY + this.nodeHeight * 2;

            var offsetX = (this.width - treeWidth) / 2 - minX + this.nodeWidth;
            var offsetY = 50;

            nodes.forEach(function (d) {
                d.x += offsetX;
                d.y += offsetY;
            });

            this.g.selectAll('.link')
                .data(links)
                .enter()
                .append('path')
                .attr('class', 'link')
                .attr('d', function (d) {
                    return 'M' + d.source.x + ',' + (d.source.y + 30)
                        + 'C' + d.source.x + ',' + ((d.source.y + d.target.y) / 2)
                        + ' ' + d.target.x + ',' + ((d.source.y + d.target.y) / 2)
                        + ' ' + d.target.x + ',' + d.target.y;
                });

            var node = this.g.selectAll('.node')
                .data(nodes)
                .enter()
                .append('g')
                .attr('class', 'node')
                .attr('transform', function (d) {
                    return 'translate(' + d.x + ',' + d.y + ')';
                });

            node.append('rect')
                .attr('class', function (d) {
                    var classes = 'node-rect';
                    if (d.data.data && d.data.data.isDeceased) {
                        classes += ' deceased';
                    }
                    return classes;
                })
                .attr('x', -this.nodeWidth / 2)
                .attr('y', 0)
                .attr('width', this.nodeWidth)
                .attr('height', this.nodeHeight)
                .attr('rx', 8)
                .attr('ry', 8);

            node.append('text')
                .attr('class', 'node-name')
                .attr('x', 0)
                .attr('y', 25)
                .attr('text-anchor', 'middle')
                .text(function (d) {
                    return d.data.name || '未知';
                });

            node.append('text')
                .attr('class', 'node-generation')
                .attr('x', 0)
                .attr('y', 45)
                .attr('text-anchor', 'middle')
                .text(function (d) {
                    if (d.data.data && d.data.data.generation) {
                        return '第' + d.data.data.generation + '世';
                    }
                    return '';
                });

            node.attr('data-id', function (d) {
                return d.data.id;
            });
        },

        /**
         * 绑定交互事件
         * 
         * 包括缩放控制、全屏切换、节点点击查看详情。
         */
        bindEvents: function () {
            var self = this;

            ChartUtils.bindZoomControls(this.svg, this.zoom);
            ChartUtils.bindFullscreen('lineage-chart-container');

            this.g.selectAll('.node').on('click', function (event, d) {
                event.stopPropagation();
                if (d.data.id && d.data.id !== 'root') {
                    ChartUtils.showMemberDetail(d.data.id);
                }
            });
        }
    };

    window.LineageChart = LineageChart;

    ChartUtils.bootstrapChart('lineage-chart-container', 'd3', function () {
        LineageChart.init();
    });
})();
