/**
 * family-tree.js - 家谱详情页主应用模块
 * 
 * 管理家谱详情页的整体交互逻辑，包括：
 * - 标签页切换和视图内容加载
 * - 成员详情弹窗的显示
 * - 成员详情信息的渲染
 * 
 * 核心机制：
 * Detail.cshtml 页面包含标签页导航和内容区域，
 * 点击不同标签时通过 AJAX 请求对应的图表页面，
 * 将返回的 HTML 内容注入到内容区域中。
 * 
 * AJAX 加载脚本的处理策略：
 * 1. 内联脚本（window.familyTreeId/membersData 赋值）通过 eval 同步执行
 * 2. 外部脚本（图表模块JS）通过动态创建 script 元素加载
 * 3. 使用 Promise 链确保脚本按顺序加载完成
 * 4. 避免重复加载已存在的脚本（通过 data-chart-script 标记）
 * 5. D3 库在 Detail.cshtml 中已加载，子页面无需重复加载
 */
(function () {
    'use strict';

    var FamilyTreeApp = {
        /** 当前激活的视图名称 */
        currentView: 'lineage',

        /** 视图实例缓存 */
        views: {},

        /**
         * 初始化应用
         * 
         * 绑定标签页切换事件和全局点击事件，然后加载默认视图（吊线图）。
         */
        init: function () {
            this.bindEvents();
            this.loadView('lineage');
        },

        /**
         * 绑定全局事件
         * 
         * 包括：
         * - 标签页按钮点击切换视图
         * - 成员节点/查看按钮点击显示详情
         */
        bindEvents: function () {
            var self = this;

            document.querySelectorAll('.tab-btn').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var view = this.getAttribute('data-view');
                    var url = this.getAttribute('data-url');

                    document.querySelectorAll('.tab-btn').forEach(function (b) {
                        b.classList.remove('active');
                    });
                    this.classList.add('active');

                    self.loadViewContent(url);
                    self.currentView = view;
                });
            });

            document.addEventListener('click', function (e) {
                if (e.target.closest('.member-node') || e.target.closest('.view-btn')) {
                    var id = e.target.closest('[data-id]')?.getAttribute('data-id');
                    if (id) {
                        self.showMemberDetail(id);
                    }
                }
            });
        },

        /**
         * 加载指定视图
         * 
         * 根据视图名称找到对应的标签按钮，获取其URL并加载内容。
         * 
         * @param {string} view - 视图名称（lineage/table-chart/tree/grid/timeline/graph）
         */
        loadView: function (view) {
            var activeBtn = document.querySelector('.tab-btn[data-view="' + view + '"]');
            if (activeBtn) {
                var url = activeBtn.getAttribute('data-url');
                this.loadViewContent(url);
            }
        },

        /**
         * 通过AJAX加载视图内容
         * 
         * 请求指定URL获取图表页面的HTML，提取其中的图表内容区域，
         * 注入到当前页面的视图容器中，并正确处理页面中的脚本。
         * 
         * 脚本处理策略：
         * 1. 提取所有 <script> 标签
         * 2. 内联脚本通过 eval 同步执行（设置全局变量等）
         * 3. 外部脚本通过动态创建 <script> 元素按顺序加载
         * 4. 已加载的脚本通过 data-chart-script 属性标记，避免重复加载
         * 5. D3 等已在主页面加载的库会被跳过
         * 
         * @param {string} url - 要加载的图表页面URL
         */
        loadViewContent: function (url) {
            var contentContainer = document.getElementById('view-content');
            if (!contentContainer) return;

            contentContainer.innerHTML = '<div class="loading-state"><div class="spinner"></div><p>加载中...</p></div>';

            fetch(url, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
            .then(function (response) {
                if (!response.ok) {
                    throw new Error('加载失败');
                }
                return response.text();
            })
            .then(function (html) {
                var parser = new DOMParser();
                var doc = parser.parseFromString(html, 'text/html');
                var pageContent = doc.querySelector('.chart-page');

                if (pageContent) {
                    contentContainer.innerHTML = pageContent.outerHTML;

                    var scripts = doc.querySelectorAll('script');
                    var inlineScripts = [];
                    var externalScripts = [];

                    scripts.forEach(function (script) {
                        if (script.src) {
                            externalScripts.push(script.src);
                        } else {
                            var text = script.textContent.trim();
                            if (text) {
                                inlineScripts.push(text);
                            }
                        }
                    });

                    inlineScripts.forEach(function (text) {
                        try {
                            eval(text);
                        } catch (e) {
                            console.error('执行内联脚本失败:', e);
                        }
                    });

                    FamilyTreeApp.loadExternalScripts(externalScripts, 0);
                } else {
                    contentContainer.innerHTML = html;
                }
            })
            .catch(function (error) {
                contentContainer.innerHTML = '<div class="error-state"><p>加载失败，请刷新页面重试</p></div>';
                console.error('加载视图失败:', error);
            });
        },

        /**
         * 按顺序加载外部脚本
         * 
         * 递归加载外部脚本列表，确保前一个脚本加载完成后再加载下一个。
         * 已加载的脚本（通过 data-chart-script 属性标记）会被跳过，
         * 避免重复加载和执行。
         * 
         * 特别处理：
         * - D3 库（d3js.org）在主页面已加载，直接跳过
         * - chart-utils.js 需要在图表模块之前加载
         * 
         * @param {Array} scripts - 外部脚本URL数组
         * @param {number} index - 当前加载的脚本索引
         */
        loadExternalScripts: function (scripts, index) {
            if (index >= scripts.length) return;

            var src = scripts[index];
            var self = this;

            if (src.includes('d3js.org') || src.includes('d3.v')) {
                self.loadExternalScripts(scripts, index + 1);
                return;
            }

            var existingScript = document.querySelector('script[src="' + src + '"]');
            if (existingScript) {
                self.loadExternalScripts(scripts, index + 1);
                return;
            }

            var newScript = document.createElement('script');
            newScript.src = src;
            newScript.setAttribute('data-chart-script', 'true');
            newScript.onload = function () {
                self.loadExternalScripts(scripts, index + 1);
            };
            newScript.onerror = function () {
                console.error('加载脚本失败:', src);
                self.loadExternalScripts(scripts, index + 1);
            };
            document.body.appendChild(newScript);
        },

        /**
         * 显示成员详情弹窗
         * 
         * 通过AJAX请求获取成员详情数据，在Bootstrap模态框中展示。
         * 加载过程中显示加载动画。
         * 
         * @param {string} memberId - 成员ID
         */
        showMemberDetail: function (memberId) {
            var modalEl = document.getElementById('memberDetailModal');
            if (!modalEl) return;

            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            var content = document.getElementById('member-detail-content');

            content.innerHTML = '<div class="loading-state"><div class="spinner"></div><p>加载中...</p></div>';
            modal.show();

            fetch('/FamilyTree/GetMemberDetail?id=' + memberId)
                .then(function (response) {
                    return response.json();
                })
                .then(function (data) {
                    if (data.success) {
                        content.innerHTML = FamilyTreeApp.renderMemberDetail(data.data);
                    } else {
                        content.innerHTML = '<div class="error-state"><p>' + (data.message || '获取成员详情失败') + '</p></div>';
                    }
                })
                .catch(function (error) {
                    content.innerHTML = '<div class="error-state"><p>加载失败，请重试</p></div>';
                    console.error('获取成员详情失败:', error);
                });
        },

        /**
         * 渲染成员详情HTML
         * 
         * 根据成员数据生成详情弹窗的HTML内容，包括：
         * - 头部区域：头像、姓名、字号、世代
         * - 基本信息区域：出生日期、逝世日期、状态、居住地、职业、排行
         * - 个人信息区域（如有）
         * - 备注区域（如有）
         * 
         * @param {Object} member - 成员数据对象
         * @returns {string} 成员详情的HTML字符串
         */
        renderMemberDetail: function (member) {
            var birthDate = member.birthDateSolar
                ? new Date(member.birthDateSolar).toLocaleDateString('zh-CN')
                : (member.birthDateLunar || '未知');

            var deathDate = member.isDeceased && member.deathDateSolar
                ? new Date(member.deathDateSolar).toLocaleDateString('zh-CN')
                : (member.deathDateLunar || '');

            return ''
                + '<div class="member-detail">'
                + '  <div class="detail-header-section">'
                + '    <div class="member-avatar">'
                + '      <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="currentColor" viewBox="0 0 16 16">'
                + '        <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>'
                + '      </svg>'
                + '    </div>'
                + '    <div class="member-basic">'
                + '      <h4 class="member-fullname">' + (member.fullName || '') + '</h4>'
                + (member.alias ? '      <p class="member-alias">字号: ' + member.alias + '</p>' : '')
                + '      <p class="member-generation">第' + (member.generation || 1) + '世' + (member.generationName ? ' · ' + member.generationName : '') + '</p>'
                + '    </div>'
                + '  </div>'
                + '  <div class="detail-section">'
                + '    <h5 class="section-title">基本信息</h5>'
                + '    <div class="info-grid">'
                + '      <div class="info-item">'
                + '        <span class="info-label">出生日期</span>'
                + '        <span class="info-value">' + birthDate + '</span>'
                + '      </div>'
                + (deathDate ? ''
                    + '      <div class="info-item">'
                    + '        <span class="info-label">逝世日期</span>'
                    + '        <span class="info-value">' + deathDate + '</span>'
                    + '      </div>' : '')
                + '      <div class="info-item">'
                + '        <span class="info-label">状态</span>'
                + '        <span class="info-value">' + (member.isDeceased ? '已故' : '在世') + '</span>'
                + '      </div>'
                + (member.residence ? ''
                    + '      <div class="info-item">'
                    + '        <span class="info-label">居住地</span>'
                    + '        <span class="info-value">' + member.residence + '</span>'
                    + '      </div>' : '')
                + (member.occupation ? ''
                    + '      <div class="info-item">'
                    + '        <span class="info-label">职业</span>'
                    + '        <span class="info-value">' + member.occupation + '</span>'
                    + '      </div>' : '')
                + (member.ranking ? ''
                    + '      <div class="info-item">'
                    + '        <span class="info-label">排行</span>'
                    + '        <span class="info-value">' + member.ranking + '</span>'
                    + '      </div>' : '')
                + '    </div>'
                + '  </div>'
                + (member.personalInfo ? ''
                    + '  <div class="detail-section">'
                    + '    <h5 class="section-title">个人信息</h5>'
                    + '    <p class="section-content">' + member.personalInfo + '</p>'
                    + '  </div>' : '')
                + (member.note ? ''
                    + '  <div class="detail-section">'
                    + '    <h5 class="section-title">备注</h5>'
                    + '    <p class="section-content">' + member.note + '</p>'
                    + '  </div>' : '')
                + (member.remarks ? ''
                    + '  <div class="detail-section">'
                    + '    <h5 class="section-title">其他备注</h5>'
                    + '    <p class="section-content">' + member.remarks + '</p>'
                    + '  </div>' : '')
                + '</div>';
        }
    };

    window.FamilyTreeApp = FamilyTreeApp;

    document.addEventListener('DOMContentLoaded', function () {
        if (document.querySelector('.detail-page')) {
            FamilyTreeApp.init();
        }
    });
})();
