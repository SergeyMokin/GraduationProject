import React from 'react';
import { AsyncStorage } from 'react-native';
import { Container, Button, Footer, FooterTab, Icon } from 'native-base';
import ApiRequsts from '../api';
import ProfilePage from './profile-page';
import BlankCreatorPage from './blank-creator-page';
import BlankListPage from './blank-list-page';
import GestureRecognizer, { swipeDirections } from 'react-native-swipe-gestures';

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

    onSwipe(direction) {
        const { SWIPE_LEFT, SWIPE_RIGHT } = swipeDirections;
        switch (direction) {
            case SWIPE_LEFT:
                {
                    if (this.state.isLoading === true) return;
                    let pageName = this.state.currentPage === MainPage.routes.main
                        ? MainPage.routes.blankCreator
                        : this.state.currentPage === MainPage.routes.blankCreator
                            ? MainPage.routes.profile
                            : MainPage.routes.main;
                    this.setState({
                        currentPage: pageName
                    });
                    break;
                }
            case SWIPE_RIGHT:
                {
                    if (this.state.isLoading === true) return;
                    let pageName = this.state.currentPage === MainPage.routes.main
                        ? MainPage.routes.profile
                        : this.state.currentPage === MainPage.routes.blankCreator
                            ? MainPage.routes.main
                            : MainPage.routes.blankCreator;
                    this.setState({
                        currentPage: pageName
                    });
                    break;
                }
        }
    }

    render() {
        const configSwipe = {
            velocityThreshold: 0.3,
            directionalOffsetThreshold: 80,
            detectSwipeUp: false,
            detectSwipeDown: false
        };

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
                <GestureRecognizer
                    config={configSwipe}
                    style={{
                        flex: 1
                    }}
                    onSwipe={(direction) => this.onSwipe(direction)}
                >
                    {content}
                </GestureRecognizer>

                <Footer disable style={{ backgroundColor: "#4a76a8", bottom: -1 }}>
                    {buttonContent}
                </Footer>
            </Container>
        )
    }
}
