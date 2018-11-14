import React from 'react';
import { AsyncStorage } from 'react-native';
import { Container, Button, Footer, FooterTab, Icon } from 'native-base';
import ApiRequsts from '../api';
import ProfilePage from './profile-page';
import BlankCreatorPage from './blank-creator-page';
import BlankListPage from './blank-list-page';

export default class MainPage extends React.Component {
    static routes = {
        main: "list",
        blankCreator: "ios-add-circle",
        profile: "person"
    };

    constructor(props) {
        super(props);

        this.state = {
            currentPage: MainPage.routes.main,
            isLoading: false
        };

        this.api = new ApiRequsts();
    }

    logout() {
        AsyncStorage.removeItem(this.api.asyncStorageUser);
        this.props.logout();
    }

    changePage(name) {
        if (this.state.isLoading === true) return;
        this.setState({
            currentPage: name
        });
    }

    renderButton(name) {
        if (this.state.currentPage === name) {
            return <Button active style={{ backgroundColor: "#4a76a8" }} onPress={() => this.changePage(name)}>
                <Icon active name={name} />
            </Button>
        }
        else {
            return <Button style={{ backgroundColor: "#4a76a8" }} onPress={() => this.changePage(name)}>
                <Icon style={{ color: '#7f96c1' }} name={name} />
            </Button>
        }
    }

    render() {
        const content = this.state.currentPage === MainPage.routes.main ?
            <BlankListPage userInfo={this.props.userInfo} footerDisableCallback={(param) => this.setState({ isLoading: param })} />

            : this.state.currentPage === MainPage.routes.profile ?
                <ProfilePage userInfo={this.props.userInfo} logoutCallback={this.logout.bind(this)} changeUserInfo={this.props.changeUserInfo} footerDisableCallback={(param) => this.setState({ isLoading: param })} />

                :
                <BlankCreatorPage userInfo={this.props.userInfo} footerDisableCallback={(param) => this.setState({ isLoading: param })} />
            ;

        const buttonContent =
            <FooterTab style={{ backgroundColor: "#4a76a8" }}>
                {this.renderButton(MainPage.routes.main)}
                {this.renderButton(MainPage.routes.blankCreator)}
                {this.renderButton(MainPage.routes.profile)}
            </FooterTab>
            ;

        return (
            <Container>
                {content}

                <Footer disable style={{ backgroundColor: "#4a76a8", bottom: -1 }}>
                    {buttonContent}
                </Footer>
            </Container>
        )
    }
}
