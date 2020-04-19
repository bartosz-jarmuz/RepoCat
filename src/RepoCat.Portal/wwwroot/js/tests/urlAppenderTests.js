/// <reference path="../../vendors/jasmine/jasmine.js"/>
/// <reference path="../site/urlParams.js"/>

describe("Url appending tests", function () {

    it("should render proper URL from repository URL", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyOne]=someValue';

        let result = updateUrl(currentURL, 'keyOne', ['someValue']);
        expect(result).toBe(expectedURL);
    });

    it("should render proper URL when other params exist", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?query=mysearchphrase';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?query=mysearchphrase&filters[keyOne]=someValue';

        let result = updateUrl(currentURL, 'keyOne', ['someValue']);
        expect(result).toBe(expectedURL);
    });

    it("should render proper URL when two items added", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyOne]=someValue&filters[keyOne]=someOtherItem';

        let result = updateUrl(currentURL, 'keyOne', ['someValue', 'someOtherItem']);
        expect(result).toBe(expectedURL);
    });

    it("should append values to URL of different property", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue&filters[keyOne]=someValue';

        let result = updateUrl(currentURL, 'keyOne', ['someValue']);
        expect(result).toBe(expectedURL);
    });

    it("should append multiple values at once to URL of different property", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue&filters[keyOne]=someValue&filters[keyOne]=banana';

        let result = updateUrl(currentURL, 'keyOne', ['someValue', 'banana']);
        expect(result).toBe(expectedURL);
    });

    it("should render proper URL with multiple filters for same property", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue&filters[keyTwo]=YetAnother';

        let result = updateUrl(currentURL, 'keyTwo', ['someOtherValue','YetAnother']);
        expect(result).toBe(expectedURL);
    });

    it("should url encode parts", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyOne]=someOtherValue';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyOne]=someOtherValue&filters[key%20Two]=Yet%26Another%20Value';

        let result = updateUrl(currentURL, 'key Two', ['Yet&Another Value']);
        expect(result).toBe(expectedURL);
    });

    it("should url encode parts when appending", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[key%20Two]=some%20OtherValue';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[key%20Two]=some%20OtherValue&filters[key%20Two]=Yet%26Another%20Value';

        let result = updateUrl(currentURL, 'key Two', ['some OtherValue', 'Yet&Another Value']);
        expect(result).toBe(expectedURL);
    });

    it("should url encode parts when removing", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[key%20Two]=some%20OtherValue';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps';

        let result = updateUrl(currentURL, 'key Two', []);
        expect(result).toBe(expectedURL);
    });

    it("should not add duplicates", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue';

        let result = updateUrl(currentURL, 'keyTwo', ['someOtherValue']);
        expect(result).toBe(expectedURL);
    });

    it("should remove only filters by specified key", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyOne]=someValue&filters[keyTwo]=someOtherValue&filters[keyTwo]=otherKeyTwoValue&filters[keyOne]=banana';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?filters[keyTwo]=someOtherValue&filters[keyTwo]=otherKeyTwoValue';

        let result = updateUrl(currentURL, 'keyOne', []);
        expect(result).toBe(expectedURL);
    });

    it("should remove only filters by specified key and not affect other params", function () {
        let currentURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?someParam=something&filters[keyOne]=someValue&filters[keyTwo]=someOtherValue&filters[keyOne]=banana&someParam=something';
        let expectedURL = 'https://localhost:44392/Repository/Test/FakeLocalApps?someParam=something&filters[keyTwo]=someOtherValue&someParam=something';

        let result = updateUrl(currentURL, 'keyOne', []);
        expect(result).toBe(expectedURL);
    });
});