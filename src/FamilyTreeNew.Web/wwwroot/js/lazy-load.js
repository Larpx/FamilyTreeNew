(function () {
    'use strict';

    var LazyLoader = {
        defaultOptions: {
            rootMargin: '50px 0px',
            threshold: 0.01,
            loadingClass: 'lazy-loading',
            loadedClass: 'lazy-loaded',
            errorClass: 'lazy-error',
            dataSrc: 'data-src',
            dataSrcset: 'data-srcset',
            placeholderSrc: 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7'
        },

        observer: null,
        options: null,

        init: function (customOptions) {
            this.options = Object.assign({}, this.defaultOptions, customOptions);

            if ('IntersectionObserver' in window) {
                this.observer = new IntersectionObserver(this.handleIntersect.bind(this), {
                    rootMargin: this.options.rootMargin,
                    threshold: this.options.threshold
                });
                this.observeElements();
            } else {
                this.loadAllImages();
            }
        },

        observeElements: function () {
            var lazyImages = document.querySelectorAll('img[' + this.options.dataSrc + '], img[' + this.options.dataSrcset + ']');
            lazyImages.forEach(function (img) {
                if (!img.classList.contains(this.options.loadedClass)) {
                    this.observer.observe(img);
                }
            }.bind(this));
        },

        handleIntersect: function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    this.loadImage(entry.target);
                    observer.unobserve(entry.target);
                }
            }.bind(this));
        },

        loadImage: function (img) {
            var src = img.getAttribute(this.options.dataSrc);
            var srcset = img.getAttribute(this.options.dataSrcset);

            if (!src && !srcset) {
                return;
            }

            img.classList.add(this.options.loadingClass);

            var tempImg = new Image();

            tempImg.onload = function () {
                if (srcset) {
                    img.srcset = srcset;
                    img.removeAttribute(this.options.dataSrcset);
                }
                if (src) {
                    img.src = src;
                    img.removeAttribute(this.options.dataSrc);
                }

                img.classList.remove(this.options.loadingClass);
                img.classList.add(this.options.loadedClass);

                img.dispatchEvent(new CustomEvent('lazyloaded', {
                    bubbles: true,
                    detail: { element: img }
                }));
            }.bind(this);

            tempImg.onerror = function () {
                img.classList.remove(this.options.loadingClass);
                img.classList.add(this.options.errorClass);

                img.dispatchEvent(new CustomEvent('lazyerror', {
                    bubbles: true,
                    detail: { element: img }
                }));
            }.bind(this);

            tempImg.src = src || this.extractFirstSrcFromSrcset(srcset);
        },

        extractFirstSrcFromSrcset: function (srcset) {
            if (!srcset) return '';
            var parts = srcset.split(',');
            if (parts.length > 0) {
                var firstPart = parts[0].trim().split(' ');
                return firstPart[0] || '';
            }
            return '';
        },

        loadAllImages: function () {
            var lazyImages = document.querySelectorAll('img[' + this.options.dataSrc + '], img[' + this.options.dataSrcset + ']');
            lazyImages.forEach(function (img) {
                this.loadImage(img);
            }.bind(this));
        },

        refresh: function () {
            if (this.observer) {
                this.observeElements();
            }
        },

        destroy: function () {
            if (this.observer) {
                this.observer.disconnect();
                this.observer = null;
            }
        }
    };

    function initLazyLoad(options) {
        LazyLoader.init(options);
        return LazyLoader;
    }

    function refreshLazyLoad() {
        LazyLoader.refresh();
    }

    window.LazyLoader = LazyLoader;
    window.initLazyLoad = initLazyLoad;
    window.refreshLazyLoad = refreshLazyLoad;

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            initLazyLoad();
        });
    } else {
        initLazyLoad();
    }

    if (typeof MutationObserver !== 'undefined') {
        var mutationObserver = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.addedNodes && mutation.addedNodes.length > 0) {
                    var hasLazyImages = false;
                    mutation.addedNodes.forEach(function (node) {
                        if (node.nodeType === 1) {
                            if (node.tagName === 'IMG' && (node.hasAttribute('data-src') || node.hasAttribute('data-srcset'))) {
                                hasLazyImages = true;
                            } else if (node.querySelectorAll) {
                                var lazyImgs = node.querySelectorAll('img[data-src], img[data-srcset]');
                                if (lazyImgs.length > 0) {
                                    hasLazyImages = true;
                                }
                            }
                        }
                    });
                    if (hasLazyImages) {
                        refreshLazyLoad();
                    }
                }
            });
        });

        mutationObserver.observe(document.body, {
            childList: true,
            subtree: true
        });
    }
})();
