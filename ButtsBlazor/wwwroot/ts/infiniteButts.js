"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var utils = require("./utils.js");
var pageManager_js_1 = require("./pageManager.js");
var options_js_1 = require("./options.js");
var buttQueue_js_1 = require("./buttQueue.js");
var InfiniteButts = /** @class */ (function () {
    function InfiniteButts($container, options) {
        var _this = this;
        this.options = $.extend({}, options_js_1.default);
        this.option(options);
        this.$container = $container;
        this.buttQueue = new buttQueue_js_1.default(this.options);
        this.pageModes = new pageManager_js_1.default(this);
        this.refreshTimer = this.options.refreshTimer;
        $(function () {
            $(document).keydown(function (e) { return _this._onKeyDown(e); });
            _this.$loader = $('<div id="spinner" class="spinner-border" role="status"></div>').prependTo($container);
            _this.$refreshBox =
                $('<span class="lb-refresh"><span>Refresh: </span></div>').appendTo($('.lb-details')).hide();
            _this.$timer = $('<span id="timer"></span>').appendTo(_this.$refreshBox);
            _this.$imageBox = $('#imageBox');
            if (_this.$imageBox.attr('href')) {
                _this.$imageBox.trigger('click');
            }
            _this.$metaTag = $('meta[property=og\\:image]');
            $('.lb-nav,#lightbox,#lightboxOverlay')
                .off('click')
                .on('click', function (e) { return _this._onClicked(e); });
            _this.loadPage(utils.getPageFromUrl(window.location.pathname));
        });
    }
    InfiniteButts.prototype.loadPage = function (page) {
        var _a;
        return __awaiter(this, void 0, void 0, function () {
            var changed, mode, _b;
            return __generator(this, function (_c) {
                switch (_c.label) {
                    case 0:
                        changed = page != ((_a = this.pageModes.current) === null || _a === void 0 ? void 0 : _a.pageName);
                        mode = this.pageModes.setPage(page).mode;
                        if (!(changed || this.pageMode != mode)) return [3 /*break*/, 3];
                        this.pageMode = mode;
                        _b = this.setPage;
                        return [4 /*yield*/, this.pageMode.first()];
                    case 1: return [4 /*yield*/, _b.apply(this, [_c.sent()])];
                    case 2:
                        _c.sent();
                        return [3 /*break*/, 5];
                    case 3: return [4 /*yield*/, this.loop()];
                    case 4:
                        _c.sent();
                        _c.label = 5;
                    case 5: return [2 /*return*/];
                }
            });
        });
    };
    InfiniteButts.prototype.loop = function () {
        return __awaiter(this, void 0, void 0, function () {
            var next;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        clearTimeout(this.timer);
                        _a.label = 1;
                    case 1:
                        _a.trys.push([1, , 4, 5]);
                        this.$loader.delay(250).show(250);
                        return [4 /*yield*/, this.pageMode.next()];
                    case 2:
                        next = _a.sent();
                        return [4 /*yield*/, this.setPage(next)];
                    case 3:
                        _a.sent();
                        return [3 /*break*/, 5];
                    case 4:
                        this.$loader.hide();
                        return [7 /*endfinally*/];
                    case 5: return [2 /*return*/];
                }
            });
        });
    };
    InfiniteButts.prototype.setPage = function (next) {
        return __awaiter(this, void 0, void 0, function () {
            var nextRefresh;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        clearTimeout(this.timer);
                        nextRefresh = this.refreshTimer;
                        if (next.page && !this.pageMode.isPageMatch(next.page)) {
                            this.loadPage(next.page);
                            return [2 /*return*/];
                        }
                        if (next.nextRefresh)
                            nextRefresh = next.nextRefresh;
                        return [4 /*yield*/, this.setButt(next)];
                    case 1:
                        _a.sent();
                        this.countdown(nextRefresh);
                        this.isRunning = true;
                        this.timer = setTimeout(function () { return __awaiter(_this, void 0, void 0, function () {
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0: return [4 /*yield*/, this.loop()];
                                    case 1:
                                        _a.sent();
                                        return [2 /*return*/];
                                }
                            });
                        }); }, nextRefresh);
                        return [2 /*return*/];
                }
            });
        });
    };
    InfiniteButts.prototype.countdown = function (timer) {
        this.countDownDate = new Date(new Date().getTime() + timer);
        if (!this.countDownInterval)
            this.startCountdown();
    };
    InfiniteButts.prototype.startCountdown = function () {
        var _this = this;
        if (!this.countDownInterval) {
            this.$refreshBox.show();
            // Update the count down every 1 second
            this.countDownInterval = setInterval(function () {
                // Get today's date and time
                var now = new Date().getTime();
                // Find the distance between now and the count down date
                var distance = _this.countDownDate.getTime() - now;
                // Time calculations for days, hours, minutes and seconds
                var seconds = Math.floor(distance / 1000);
                if (seconds < 0)
                    seconds = 0;
                // Display the result in the element with id="demo"
                _this.$timer.text(seconds + 's');
                // If the count down is finished, write some text
                _this.$refreshBox.toggle(_this.refreshTimer > 2000);
            }, 1000);
        }
    };
    InfiniteButts.prototype.stopCountdown = function () {
        if (this.countDownInterval) {
            clearInterval(this.countDownInterval);
            this.$refreshBox.hide();
            this.countDownInterval = null;
        }
    };
    InfiniteButts.prototype.setButt = function (butt) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                if (!butt.page)
                    butt.page = butt.data.index.toString();
                this.pageModes.setButtPage(butt);
                this.$imageBox.attr("data-title", butt.title);
                if (!this.image || butt.data.path !== this.image.path) {
                    this.image = butt.data;
                    this.$imageBox.attr('href', this.image.path);
                    this.$metaTag.attr('content', this.image.path);
                    this.$imageBox.trigger('click');
                }
                return [2 /*return*/, this.image];
            });
        });
    };
    InfiniteButts.prototype._onKeyDown = function (event) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(event.key == "ArrowRight" || event.key == " " || event.key == "Spacebar")) return [3 /*break*/, 2];
                        return [4 /*yield*/, this.next(event)];
                    case 1:
                        _a.sent();
                        return [3 /*break*/, 3];
                    case 2:
                        if (event.key == "ArrowLeft") {
                            this.prev();
                        }
                        _a.label = 3;
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    InfiniteButts.prototype._onClicked = function (event) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.next(event)];
                    case 1:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    InfiniteButts.prototype.prev = function () {
        if (this.lastClick && (new Date().getTime() - this.lastClick.getTime()) < this.options.maxNavSpeed)
            return;
        this.lastClick = new Date();
        history.back();
    };
    InfiniteButts.prototype.next = function (event) {
        var _a;
        return __awaiter(this, void 0, void 0, function () {
            var page;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.lastClick && (new Date().getTime() - this.lastClick.getTime()) < this.options.maxNavSpeed)
                            return [2 /*return*/];
                        this.lastClick = new Date();
                        if (!((_a = this.pageModes.current) === null || _a === void 0 ? void 0 : _a.mode)) return [3 /*break*/, 3];
                        return [4 /*yield*/, this.pageModes.current.mode.clicked(event)];
                    case 1:
                        page = _b.sent();
                        if (!page) return [3 /*break*/, 3];
                        return [4 /*yield*/, this.setPage(page)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    InfiniteButts.prototype.option = function (options) {
        $.extend(this.options, options);
    };
    return InfiniteButts;
}());
exports.default = InfiniteButts;
