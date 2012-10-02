$(function () {

    window.App = {};

    App.ChecklistItem = Backbone.Model.extend({
        toggleDone: function () {
            var actuallyDone = !this.get('IsCompleted');
            this.save({ IsCompleted: actuallyDone });
        },
        validate: function (changes) {
            if (changes.Content != null && changes.Content == "") {
                return "Items must have content, or the zombies will eat your brains";
            }
        }
    });

    App.ChecklistItemCollection = Backbone.Collection.extend({
        model: App.ChecklistItem,
        url: '/services/zac/items',
        comparator: function (model) {
            return model.get('IsCompleted');
        }
    });

    App.AppView = Backbone.View.extend({
        el: $('#application'),
        events: {
            'click #submit-new-item': 'submitNewItem'
        },
        initialize: function () {
            this.checklistItemCollection = new App.ChecklistItemCollection();
            this.checklistItemCollection.bind('reset', this.addAllItems, this);
            this.checklistItemCollection.fetch();
        },
        addAllItems: function () {
            var self = this;
            alert('Got ' + this.checklistItemCollection.length + ' items from API');
            this.checklistItemCollection.each(function (item) {
                self.addItem(item);
            });
        },
        submitNewItem: function () {
            var newItem = {
                Content: this.$('#new-item-content').val(),
                IsCompleted: false
            };
            var itemModel = this.checklistItemCollection.create(newItem, {
                error: function (model, error) {
                    alert("Unable to create new item: " + error);
                }
            });
            this.addItem(itemModel);
            this.$('#new-item-content').val('');
        },
        addItem: function (item) {
            var view = new App.ChecklistItemView({ model: item });
            this.$('#item-list').append(view.render().el);
        }
    });

    App.ChecklistItemView = Backbone.View.extend({
        tagName: 'div',
        events: {
            'change .isDone': 'toggleDone',
            'click .remove-item': 'removeItem'
        },
        initialize: function () {
            this.model.bind('change', this.render, this);
            this.model.bind('destroy', this.remove, this);
        },
        template: _.template($('#item-template').html()),
        render: function () {
            this.$el.html(this.template(this.model.toJSON()));
            return this;
        },
        toggleDone: function () {
            this.model.toggleDone();
        },
        removeItem: function () {
            this.model.destroy();
        }
    });

    window.appView = new App.AppView();
});
