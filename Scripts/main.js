
class Book {

    // private attributes

    #controller;
    #view;
    #id;
    #query;
    #path;
    #iconAsc;
    #iconDesc;
    #sortBy;
    #isDesc;

    #apiHandler;

    constructor() {
        this.#controller = window.location.pathname.split("/")[1];
        this.#view = window.location.pathname.split("/")[2];
        this.#id = window.location.pathname.split("/")[3];
        this.#query = window.location.search;
        this.#path = window.location.pathname;

        this.#iconAsc = "&#9652;";
        this.#iconDesc = "&#9662;";

        this.#sortBy = 0;
        this.#isDesc = true;

        this.#apiHandler = {
            DELETE(controller, id) {
                if (confirm("This action will delete all relationships existing with the selected item. Are you sure you want to proceed?")) {
                    $.ajax({
                        url: `/${controller}/Delete/${id}`,
                        type: 'GET',
                        success: function (res) {
                            alert(`${controller} deleted`);
                            if (controller === 'InvoiceLineItem') {
                                window.location.href = `/Invoice/Upsert/${id.split("-")[0]}`;
                            }
                            else {
                                window.location.href = `/${controller}/All`;
                            }
                            
                        }
                    });
                } else {
                    alert("Luck we asked!");
                }
            },
        };
    }

    // public methods

    // handler the method selected
    ApiHandler(method, id) {
        this.#apiHandler.DELETE(this.#controller, id);
    }

    // sort the list
    Sort() {
        if (this.#isSortOrSearch()) {
            let th = document.querySelectorAll("th");

            if (this.#query === "") th[0].innerHTML += this.#iconAsc;
            else {
                this.#sortBy = (new URLSearchParams(this.#query)).get("sortBy");
                this.#isDesc = (new URLSearchParams(this.#query)).get("isDesc");

                th[this.#sortBy].innerHTML += (this.#isDesc === "true" ? this.#iconDesc : this.#iconAsc);
            }

            this.#LoadHeaderEvents(th);
        }
    };

    // change the title of the upsert page
    UpsertTitle() {
        if (this.#view === "Upsert") {
            let action = (this.#id === "0" ? "Add" : "Edit");
            document.querySelector("#title").innerHTML = action;
        }
    };

    // perform a search on the list view
    Search() {
        if (this.#isSortOrSearch()) {
            let searchTerm = document.querySelector("#txtSearch").value;

            if (this.#controller === 'InvoiceLineItem') {
                let invoiceID = this.#query.split("=")[1];
                invoiceID = invoiceID.split("&")[0];
                searchTerm = searchTerm.replace(".", "-");
                searchTerm = "?invoiceID=" + invoiceID + "&id=" + searchTerm;
            }

            window.location.href = `/${this.#controller}/${this.#view}/${searchTerm}`;
        }
    };

    // redirect to the upsert view
    Upsert(id) {
        //if (id === undefined) id = 0;
        if (id === undefined) {
            id = 0;
        }

        if (this.#controller === 'InvoiceLineItem') {
            let invoiceID = this.#query.split("=")[1];
            invoiceID = invoiceID.split("&")[0];
            id = invoiceID + "-" + id;
            
            //document.querySelector("#invoiceID").innerHTML = invoiceID;
        }

        window.location.href = `/${this.#controller}/Upsert/${id}`;
    };

    // back to the list view
    Back() {
        let invoiceID = "";

        if (this.#controller === 'InvoiceLineItem') {
            

            if (this.#view === "Upsert") {
                invoiceID = this.#path.split("/")[3];
                invoiceID = "?invoiceID=" + invoiceID.split("-")[0];
            } else {
                invoiceID = this.#query.split("=")[1];
                return window.location.href = `/Invoice/Upsert/${invoiceID}`;
            }
        }

        window.location.href = `/${this.#controller}/All/${invoiceID}`;
    };

    // get the id of the invoice
    GetInvoiceId() {
        let invoiceID = ""
        if (this.#view === "All") {
            invoiceID = this.#query.split("=")[1];
            invoiceID = invoiceID.split("&")[0];
            document.querySelector("#invoiceId").innerHTML = invoiceID;
        } else {
            invoiceID = this.#path.split("/")[3];
            invoiceID = invoiceID.split("-")[0];
            document.querySelector("#invoiceId").value = invoiceID;
        }
    }


    // private methods

    // verify if the view == all, to allow the sort and search fields
    #isSortOrSearch() {
        //if (this.#path !== "/" && this.#view === "All" && this.#controller !== "InvoiceLineItem") return true;
        if (this.#path !== "/" && this.#view === "All") return true;

        return false;
    }

    // show the icon in front of the header choose to be ordered
    #LoadHeaderEvents(th) {
        th.forEach((element, index, array) => {
            if ((array.length - 1) !== index) {

                element.addEventListener("click", () => {

                    if (this.#query === "") {
                        this.#isDesc = true;
                    } else {
                        this.#isDesc = ((new URLSearchParams(this.#query)).get("isDesc") === "true" ? false : true);
                    }
                    window.location.href = `${this.#path}?sortBy=${index}&isDesc=${this.#isDesc}`;
                });
            }
        });
    }
}

(function () {
    new Book().Sort();
    new Book().UpsertTitle();
})();